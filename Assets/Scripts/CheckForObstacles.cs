using UnityEngine;

/*
 * The reason for this class is that a Kinematic rigidbody does not get OnCollisionEnter callbacks in Unity, only OnTriggerEnter
 * and in the canyon level with the non-convex meshes, the mesh colliders can't be triggers
 *
 * So we'll implement our own collision detection using Physics.OverlapBox (since we know the player has a box collider)
 */
[RequireComponent(typeof(BoxCollider), typeof(PlayerMovement))]
public class CheckForObstacles : MonoBehaviour
{
    private BoxCollider boxCollider;
    private PlayerMovement playerMovement;
    private Collider[] colliderCache;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        playerMovement = GetComponent<PlayerMovement>();
        colliderCache = new Collider[10];
    }

    private void FixedUpdate()
    {
        var t = transform;
        var boxCenter = t.TransformPoint(boxCollider.center);
        var halfExtents = boxCollider.size * 0.5f;
        
        var hits = Physics.OverlapBoxNonAlloc(
            boxCenter, 
            halfExtents, 
            colliderCache,
            t.rotation);
        
        for (var i = 0; i < hits; i++)
        {
            if (colliderCache[i] == boxCollider) continue; // Skip the inevitable self-collision from the OverlapBox call
            
            if (playerMovement.CheckCollision(colliderCache[i]))
            {
                // Hacky solution to make sure player can only collide with a specific obstacle ONE time :)
                colliderCache[i].enabled = false;
            }
        }
    }
}