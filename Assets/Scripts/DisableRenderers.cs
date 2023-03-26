using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderers : MonoBehaviour
{
        public Transform playerTransform;
    public float disableDistance = 100f;

    private Renderer[] renderers;
    private bool[] originalRendererStates;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalRendererStates = new bool[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalRendererStates[i] = renderers[i].enabled;
        }
    }

    private void Update()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (Vector3.Distance(renderers[i].transform.position, playerTransform.position) > disableDistance)
            {
                renderers[i].enabled = false;
            }
            else
            {
                renderers[i].enabled = originalRendererStates[i];
            }
        }
    }
}
