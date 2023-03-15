using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BakeBarycentricCoordinatesFilter : MonoBehaviour
{
    private void Awake()
    {
        var filter = GetComponent<MeshFilter>();
        filter.mesh = filter.mesh.WithBarycentricCoordinates();
    }
}