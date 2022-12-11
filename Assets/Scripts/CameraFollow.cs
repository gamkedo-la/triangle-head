using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothTime = 0.3F;
    public Vector2 limits;
    private Vector3 velocity = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -limits.x, limits.x), Mathf.Clamp(localPos.y, -limits.y, limits.y), localPos.z);   
        //look at his FollowTarget code on line 50 re: offset
    }

    /*

    public void FollowTarget(Transform t){
        Vector3 localPos = transform.localPosition;
        Vector3 targetLocalPos = t.transform.localPosition;
        //transform.localPosition = Vector3.SmoothDamp(localPos, new Vector3(targetLocalPos.x + offset.x, targetLocalPos.y + offset.y, ))
        //just look up SmoothDamp in the docs, see what the example code looks like, can you adapt it?
    }
    */
}
