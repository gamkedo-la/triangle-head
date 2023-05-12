using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class MeshHelper
{
    public static Mesh WithBarycentricCoordinates(this Mesh mesh)
    {
        if (mesh.HasVertexAttribute(VertexAttribute.TexCoord2))
        {
            Debug.Log($"Mesh {mesh.name} already has uv2 baked into it, will not apply barycentric coordinates there");
            return mesh;
        }

        var hasTangents = mesh.HasVertexAttribute(VertexAttribute.Tangent);
        var hasUv1 = mesh.HasVertexAttribute(VertexAttribute.TexCoord1);

        var numberOfSubMeshes = mesh.subMeshCount;
        int[][] oldTrianglesArray = new int[numberOfSubMeshes][];
        int[][] newTrianglesArray = new int[numberOfSubMeshes][];
        var numberOfVertices = 0;
        for (var subMeshIndex = 0; subMeshIndex < numberOfSubMeshes; subMeshIndex++)
        {
            oldTrianglesArray[subMeshIndex] = mesh.GetTriangles(subMeshIndex);
            numberOfVertices += oldTrianglesArray[subMeshIndex].Length;
        }
        
        var barycentric = new Vector3[numberOfVertices];
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

        var vertexBaseIndex = 0;
        for (var subMeshIndex = 0; subMeshIndex < numberOfSubMeshes; subMeshIndex++)
        {
            var oldTriangles = oldTrianglesArray[subMeshIndex];
            var numberOfSubMeshVertices = oldTriangles.Length;
            var triangles = newTrianglesArray[subMeshIndex] = new int[numberOfSubMeshVertices];
            // Need to make each vertex unique in order to properly interpolate over barycentric coordinates
            // (What the geometry shader did, but here we bake it in the mesh instead)
            for (var i = 0; i < numberOfSubMeshVertices; i += 3)
            {
                for (var vi = i; vi < i + 3; vi++)
                {
                    var oldIndex = oldTriangles[vi];

                    triangles[vi] = vi + vertexBaseIndex;
                    vertices[vi + vertexBaseIndex] = oldVertices[oldIndex];
                    uv0[vi + vertexBaseIndex] = oldUv0[oldIndex];
                    if (hasUv1)
                    {
                        uv1[vi + vertexBaseIndex] = oldUv1[oldIndex];
                    }

                    normals[vi + vertexBaseIndex] = oldNormals[oldIndex];
                    if (hasTangents)
                    {
                        tangents[vi + vertexBaseIndex] = oldTangents[oldIndex];
                    }
                }

                barycentric[i + vertexBaseIndex] = Vector3.right;
                barycentric[i + 1 + vertexBaseIndex] = Vector3.up;
                barycentric[i + 2 + vertexBaseIndex] = Vector3.forward;
            }

            vertexBaseIndex += numberOfSubMeshVertices;
        }

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
        
        for (var subMeshIndex = 0; subMeshIndex < numberOfSubMeshes; subMeshIndex++)
        {
            mesh.SetTriangles(newTrianglesArray[subMeshIndex], subMeshIndex);
        }

        return mesh;
    }
}