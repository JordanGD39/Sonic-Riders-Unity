using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Transform playerTransform;
    private PlayerFlight playerFlight;
    private PlayerPunchObstacle playerPunch;
    private AudioManagerHolder audioHolder;
    private Rigidbody rb;
    private CharacterStats charStats;

    [SerializeField] private float speed = 2;
    private float closestDistance;
    [SerializeField] private float time = 0.5f;
    private Vector3 bounceDir;

    private PathCreator path;
    private float autoSpeed = 0;
    private float launchSpeed = 0;
    private Transform launchForward;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerTransform = playerMovement.transform;
        playerFlight = playerMovement.GetComponent<PlayerFlight>();
        playerPunch = playerMovement.GetComponent<PlayerPunchObstacle>();
        charStats = playerMovement.GetComponent<CharacterStats>();
        audioHolder = playerMovement.GetComponent<AudioManagerHolder>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.layer)
        {            
            case 0:
                if (!collision.isTrigger)
                {
                    BounceCol(collision);
                }                
                break;
            case 10:
                if (playerFlight.enabled)
                {
                    playerFlight.IncreaseFlightSpeed(collision.transform.parent);
                }
                break;
            case 11:
                playerPunch.Punch(collision.GetComponentInParent<Rigidbody>());

                if (playerPunch.CantPunch || charStats.Air <= 0)
                {
                    BounceCol(collision);
                }
                break;
            case 13:
                if (!collision.isTrigger)
                {
                    BounceCol(collision);
                }
                break;
            case 14:
                if (charStats.Air == 0 && path == null)
                {
                    Transform spring = collision.transform.parent;
                    path = spring.GetComponentInChildren<PathCreator>();

                    SpringStats springStats = spring.GetComponent<SpringStats>();

                    autoSpeed = springStats.Speed;
                    launchForward = springStats.Forward;
                    launchSpeed = springStats.LaunchSpeed;

                    StartCoroutine("FollowPath");
                }
                break;
        }       
    }

    private IEnumerator FollowPath()
    {
        if (path != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            playerMovement.CantMove = true;
            speed = playerMovement.Speed;
            closestDistance = path.path.GetClosestDistanceAlongPath(playerTransform.position);
            Quaternion rot = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);
            rot.x = 0;
            rot.z = 0;
            playerTransform.GetChild(0).localRotation = rot;
            playerTransform.position = path.path.GetClosestPointOnPath(playerTransform.position);

            while (path.path.GetClosestTimeOnPath(playerTransform.position) < 0.99f)
            {
                closestDistance += autoSpeed * Time.deltaTime;
                Vector3 desiredPos = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
                //desiredPos += transform.GetChild(0).TransformDirection(0, extraCharHeight, 0);
                playerTransform.position = desiredPos;

                Quaternion localRot = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);
                localRot.x = 0;
                localRot.z = 0;
                playerTransform.GetChild(0).localRotation = localRot;
                yield return null;
            }

            path = null;

            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            playerTransform.GetChild(0).forward = launchForward.forward;
            playerTransform.GetChild(0).localRotation = new Quaternion(0, playerTransform.GetChild(0).localRotation.y, 0, playerTransform.GetChild(0).localRotation.w);

            rb.velocity = launchForward.forward * launchSpeed;

            playerMovement.CantMove = false;
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

        //Debug.Log(transform.InverseTransformDirection(bounceDir).z);

        bool hitDirectly = false;

        if (Mathf.Abs(transform.InverseTransformDirection(bounceDir).z) > 0.8f)
        {
            playerMovement.Speed = speed;
            hitDirectly = true;
        }

        audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);

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
