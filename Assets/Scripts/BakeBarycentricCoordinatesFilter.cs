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

        var oldTriangles = mesh.triangles;
        var numberOfVertices = oldTriangles.Length; 

        var barycentric = new Vector3[numberOfVertices];
        var triangles = new int[numberOfVertices];
        var vertices = new Vector3[numberOfVertices];
        var uv0 = new Vector2[numberOfVertices];
        var uv1 = new Vector2[numberOfVertices];
        var normals = new Vector3[numberOfVertices];
        var tangents = new Vector4[numberOfVertices];

        var oldNumberOfVertices = mesh.vertexCount;
        var oldVertices = new List<Vector3>(oldNumberOfVertices);
        mesh.GetVertices(oldVertices);
        var oldUv0 = new List<Vector2>(oldNumberOfVertices);
        mesh.GetUVs(0, oldUv0);
        var oldUv1 = new List<Vector2>(oldNumberOfVertices);
        mesh.GetUVs(1, oldUv1);
        var oldNormals = new List<Vector3>(oldNumberOfVertices);
        mesh.GetNormals(oldNormals);
        var oldTangents = new List<Vector4>(oldNumberOfVertices);
        mesh.GetTangents(oldTangents);

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
                uv1[vi] = oldUv1[oldIndex];
                normals[vi] = oldNormals[oldIndex];
                tangents[vi] = oldTangents[oldIndex];
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
        mesh.SetTangents(tangents);
        mesh.SetUVs(0, uv0);
        mesh.SetUVs(1, uv1);
        mesh.SetUVs(2, barycentric);
        mesh.SetTriangles(triangles, 0);
        mesh.UploadMeshData(true);
        filter.mesh = mesh;
    }
}