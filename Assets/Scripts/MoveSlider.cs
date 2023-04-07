using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlider : MonoBehaviour
{
    public float sliderSpeed;
    public float sliderDistance;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;

    void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + new Vector3(sliderDistance, 0f, 0f);
    }

    void Update()
    {
        Vector3 targetPosition;

        // Determine the target position based on whether the object is moving towards its end position or back to its start position
        if (movingToEnd)
        {
            targetPosition = endPosition;
        }
        else
        {
            targetPosition = startPosition;
        }

        // Move the object towards the target position at the specified speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, sliderSpeed * Time.deltaTime);

        // If the object reaches the target position, toggle the movingToEnd flag to change direction
        if (transform.position == targetPosition)
        {
            movingToEnd = !movingToEnd;
        }
    }
}