using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerFlight playerFlight;
    private PlayerPunchObstacle playerPunch;
    private Rigidbody rb;
    [SerializeField] private float speed = 2;
    [SerializeField] private float time = 0.5f;
    private Vector3 bounceDir;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerFlight = playerMovement.GetComponent<PlayerFlight>();
        playerPunch = playerMovement.GetComponent<PlayerPunchObstacle>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.layer)
        {            
            case 0:
                BounceCol(collision);
                break;
            case 10:
                if (playerFlight.enabled)
                {
                    playerFlight.IncreaseFlightSpeed(collision.transform.parent);
                }
                break;
            case 11:
                playerPunch.Punch(collision.GetComponentInParent<Rigidbody>());

                if (playerPunch.CantPunch)
                {
                    BounceCol(collision);
                }
                break;
        }       
    }

    private void BounceCol(Collider collision)
    {
        playerMovement.Bouncing = true;

        Vector3 closestPoint = Vector3.zero;

        if (collision.GetComponent<MeshCollider>() != null)
        {
            closestPoint = NearestVertexTo(transform.position + transform.TransformPoint(new Vector3(0, 0, -1)), collision.GetComponent<MeshFilter>().mesh);
        }
        else
        {
            closestPoint = collision.ClosestPoint(transform.position);
        }

        bounceDir = (transform.position - closestPoint).normalized;
        StartCoroutine("Bounce");
    }

    private IEnumerator Bounce()
    {
        bool wasGrounded = playerMovement.Grounded;

        playerMovement.CantMove = true;

        Debug.Log(transform.InverseTransformDirection(bounceDir).z);

        bool hitDirectly = false;

        if (Mathf.Abs(transform.InverseTransformDirection(bounceDir).z) > 0.8f)
        {
            playerMovement.Speed = speed;
            hitDirectly = true;
        }

        rb.AddForce(bounceDir * speed, ForceMode.Impulse);
        yield return new WaitForSeconds(time / 2);
        playerMovement.Bouncing = false;
        yield return new WaitForSeconds(time / 2);
        playerMovement.CantMove = false;

        if (hitDirectly && wasGrounded)
        {
            playerMovement.Speed = 0;
            rb.velocity = Vector3.zero;
        }
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
