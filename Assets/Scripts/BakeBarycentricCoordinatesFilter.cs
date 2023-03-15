using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class BakeBarycentricCoordinatesFilter : MonoBehaviour
{
    private void Awake()
    {
        var filter = GetComponent<MeshFilter>();
        var mesh = filter.mesh;
        if (mesh.HasVertexAttribute(VertexAttribute.TexCoord2))
        {
            Debug.Log($"Mesh {mesh.name} already has uv2 baked into it, will not apply barycentric coordinates there");
            return;
        }

        var hasTangents = mesh.HasVertexAttribute(VertexAttribute.Tangent);
        var hasUv1 = mesh.HasVertexAttribute(VertexAttribute.TexCoord1);

        var oldTriangles = mesh.triangles;
        var numberOfVertices = oldTriangles.Length; 

        var barycentric = new Vector3[numberOfVertices];
        var triangles = new int[numberOfVertices];
        var vertices = new Vector3[numberOfVertices];
        var uv0 = new Vector2[numberOfVertices];
        var uv1 = hasUv1 ? new Vector2[numberOfVertices] : null;
        var normals = new Vector3[numberOfVertices];
        var tangents = hasTangents ? new Vector4[numberOfVertices] : null;

        var oldNumberOfVertices = mesh.vertexCount;
        var oldVertices = new List<Vector3>(oldNumberOfVertices);
        mesh.GetVertices(oldVertices);
        var oldUv0 = new List<Vector2>(oldNumberOfVertices);
        mesh.GetUVs(0, oldUv0);
        List<Vector2> oldUv1 = null;
        if (hasUv1)
        {
            oldUv1 = new List<Vector2>(oldNumberOfVertices);
            mesh.GetUVs(1, oldUv1);
        }

        var oldNormals = new List<Vector3>(oldNumberOfVertices);
        mesh.GetNormals(oldNormals);
        List<Vector4> oldTangents = null;
        if (hasTangents)
        {
            oldTangents = new List<Vector4>(oldNumberOfVertices);
            mesh.GetTangents(oldTangents);
        }


        // Need to make each vertex unique in order to properly interpolate over barycentric coordinates
        // (What the geometry shader did, but here we bake it in the mesh instead)
        for (var i = 0; i < numberOfVertices; i += 3)
        {
            for (var vi = i; vi < i + 3; vi++)
            {
                var oldIndex = oldTriangles[vi];

                triangles[vi] = vi;
                vertices[vi] = oldVertices[oldIndex];
                uv0[vi] = oldUv0[oldIndex];
                if (hasUv1)
                {
                    uv1[vi] = oldUv1[oldIndex];
                }

                normals[vi] = oldNormals[oldIndex];
                if (hasTangents)
                {
                    tangents[vi] = oldTangents[oldIndex];
                }
            }

            barycentric[i] = Vector3.right;
            barycentric[i + 1] = Vector3.up;
            barycentric[i + 2] = Vector3.forward;
        }

        mesh = new Mesh
        {
            name = mesh.name + " with barycentric"
        };
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        if (hasTangents)
        {
            mesh.SetTangents(tangents);
        }

        mesh.SetUVs(0, uv0);
        if (hasUv1)
        {
            mesh.SetUVs(1, uv1);
        }

        mesh.SetUVs(2, barycentric);
        mesh.SetTriangles(triangles, 0);
        mesh.UploadMeshData(true);
        filter.mesh = mesh;
    }
}