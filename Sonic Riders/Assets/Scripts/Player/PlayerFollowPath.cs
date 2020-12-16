using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerFollowPath : MonoBehaviour
{
    private float closestDistance = 0;
    public float ClosestDistance { get { return closestDistance; } set { closestDistance = value; } }
    private Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } }
    private Vector3 previousPos;
    private PlayerMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();   
    }

    public void FollowPath(VertexPath vertexPath, bool physics, float extraCharHeight)
    {
        Vector3 distance = transform.position - previousPos;

        velocity = distance / Time.deltaTime;
        previousPos = transform.position;

        if (physics)
        {
            movement.Speed += 20 * transform.GetChild(0).localRotation.x * Time.deltaTime;

            if (movement.Speed <= 4 && movement.Speed >= 0)
            {
                movement.Speed = -4;
            }
        }

        closestDistance += movement.Speed * Time.deltaTime;
        Vector3 desiredPos = vertexPath.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
        desiredPos += transform.GetChild(0).up * extraCharHeight;
        transform.position = desiredPos;
        transform.GetChild(0).localRotation = vertexPath.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);
    }
}
