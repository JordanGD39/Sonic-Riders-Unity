using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    [SerializeField] private float speed = 2;
    [SerializeField] private float time = 0.5f;
    private Vector3 bounceDir;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 0)
        {
            playerMovement.Bouncing = true;

            Vector3 closestPoint = Vector3.zero;

            if (collision.GetComponent<MeshCollider>() != null)
            {

                closestPoint = NearestVertexTo(transform.position + transform.TransformPoint(new Vector3(0,0,-1)), collision.GetComponent<MeshFilter>().mesh);
            }
            else
            {
                closestPoint = collision.ClosestPoint(transform.position);
            }

            bounceDir = (transform.position - closestPoint).normalized;            
            StartCoroutine("Bounce");
        }        
    }

    private IEnumerator Bounce()
    {
        bool wasGrounded = playerMovement.Grounded;

        playerMovement.Speed = speed;
        yield return null;
        playerMovement.CantMove = true;
        rb.velocity = bounceDir * speed;
        yield return new WaitForSeconds(time / 2);
        playerMovement.Bouncing = false;
        yield return new WaitForSeconds(time / 2);        
        playerMovement.Speed = 0;
        playerMovement.CantMove = false;
    }

    public Vector3 NearestVertexTo(Vector3 point, Mesh mesh)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return transform.TransformPoint(nearestVertex);
    }
}
