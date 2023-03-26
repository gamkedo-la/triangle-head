using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderers : MonoBehaviour
{
    public Transform playerTransform;
    public float disableDistance = 10f;

    private Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        foreach (Renderer renderer in renderers)
        {
            if (Vector3.Distance(renderer.transform.position, playerTransform.position) > disableDistance)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
    }
}
