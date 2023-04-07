using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlider : MonoBehaviour
{
    public float sliderSpeed;
    public float sliderDistance;
    private Vector3 startPosition;
    private float distanceTraveled;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate how far the object has traveled from its starting point
        distanceTraveled = Mathf.Abs(transform.position.x - startPosition.x);

        // Move the object along its x axis at the specified speed
        transform.Translate(Vector3.right * sliderSpeed * Time.deltaTime);

        // If the object has traveled the specified distance, reset it to its starting point
        if (distanceTraveled >= sliderDistance)
        {
            transform.position = startPosition;
            distanceTraveled = 0f;
        }
    }
}