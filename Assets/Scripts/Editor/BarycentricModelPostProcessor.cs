using UnityEditor;
using UnityEngine;

public class BarycentricModelPostProcessor : AssetPostprocessor
{
    private void OnPostprocessModel(GameObject g)
    {
        Apply(g.transform);
    }

    private static void Apply(Transform transform)
    {
        if (transform.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilter.sharedMesh = meshFilter.sharedMesh.WithBarycentricCoordinates();
        }

        foreach (Transform childTransform in transform)
        {
            Apply(childTransform);
        }
    }
}