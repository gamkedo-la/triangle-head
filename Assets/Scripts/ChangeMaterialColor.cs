using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialColor : MonoBehaviour
{
    public Color newColor; // The new color you want to set

    // Reference to the Renderer component of the object
    private Renderer renderer;

    private void Start()
    {
        // Get the Renderer component attached to the object
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.red);
    }

}
