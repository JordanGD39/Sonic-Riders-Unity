using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggPawnAI : MonoBehaviour
{
    private Vector3 startPos;

    private Rigidbody rb;
    //private EggPawnWalkList eggPawnWalkList;
    private Animator anim;
    private Collider areaToWalk;

    private bool grounded = false;

    [Header("Editable vars")]
    [SerializeField] private float speed = 0;
    [SerializeField] private float walkSpeed = 8;
    [SerializeField] private float runSpeed = 16;
    [SerializeField] private float rotateSpeed = 20;
    [SerializeField] private int currSpot = 0;
    [SerializeField] private float distanceToCheckSpot = 10;
    [SerializeField] private float deathY = -10;
    [SerializeField] private float spawnY = 30;
    [SerializeField] private float groundedRadius = 4;

    [Header("Search spot")]
    //Random Spot
    [SerializeField] private LayerMask raycastLayerMask;
    private bool searchingSpot = false;
    private bool foundSpot = false;
    private Vector3 spot;

    [Header("Walk towards spot")]
    //Walking towards spot
    [SerializeField] private int checkPoint = 0;
    [SerializeField] private int nextCheckPoint = 1;
    [SerializeField] private int spotCheckPoint = 0;
    private Transform checkpointTransform;
    private bool searchingCheckPointSpot = false;
    private bool foundCheckPointSpot = false;
    private Vector3 targetPos;
    private bool startWalking = false;

    //Punched
    private bool punched = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //eggPawnWalkList = FindObjectOfType<EggPawnWalkList>();
        areaToWalk = GameObject.FindGameObjectWithTag(Constants.Tags.eggPawnArea).GetComponent<Collider>();
        checkpointTransform = GameObject.FindGameObjectWithTag(Constants.Tags.eggPawnCheckPoints).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.isKinematic)
        {
            return;
        }

        if (transform.position.y < deathY)
        {
            Respawn();
        }

        grounded = Physics.CheckSphere(transform.position, groundedRadius, raycastLayerMask);

        anim.SetBool("Grounded", grounded);

        if (punched)
        {
            return;
        }

        anim.SetFloat("Speed", speed);        

        Patrolling();
    }

    public void Die()
    {
        punched = true;
        Invoke("Respawn", 3);
    }

    private void Respawn()
    {
        punched = false;
        transform.position = new Vector3(startPos.x, spawnY, startPos.z);
    }

    private void Patrolling()
    {
        if (!foundSpot && !searchingSpot)
        {
            speed = 0;
            SearchRandomSpot();
        }
        else if (foundSpot)
        {
            if (checkPoint == spotCheckPoint)
            {
                if (!startWalking)
                {
                    LookAtTarget(spot);
                }

                //Debug.Log("Distance to spot: " + (spot - transform.position).sqrMagnitude);

                if ((spot - transform.position).sqrMagnitude < distanceToCheckSpot)
                {
                    foundSpot = false;
                    speed = 0;
                    startWalking = false;
                    return;
                }
            }
            else
            {
                if (!foundCheckPointSpot && !searchingCheckPointSpot)
                {
                    NextCheckPointSpot();
                }

                if (!startWalking)
                {
                    LookAtTarget(targetPos);
                }

                if ((targetPos - transform.position).sqrMagnitude < distanceToCheckSpot)
                {
                    checkPoint = nextCheckPoint;
                    CalcNextCheckPoint();
                    speed = 0;
                    startWalking = false;
                    return;
                }
            }

            if (startWalking)
            {
                speed = walkSpeed;
            }
        }
    }

    private void SearchRandomSpot()
    {
        if (areaToWalk == null)
        {
            Debug.LogError("There is no assigned walkable area for eggpawn: " + gameObject.name);
            return;
        }

        foundSpot = false;
        searchingSpot = true;

        /*int neighbour = currSpot + Random.Range(-1, 2);

        if (neighbour < 0)
        {
            neighbour = 1;
        }
        else if (neighbour >= eggPawnWalkList.ColliderBoundsList.Count)
        {
            neighbour = eggPawnWalkList.ColliderBoundsList.Count - 2;

            if (neighbour < 0)
            {
                neighbour = 0;
            }
        }

        Collider chosenCollider = eggPawnWalkList.ColliderBoundsList[neighbour];*/

        float x = areaToWalk.bounds.extents.x;
        float z = areaToWalk.bounds.extents.z;

        Vector3 pivot = areaToWalk.transform.GetChild(0).position;

        Vector3 potentialSpot = new Vector3(pivot.x + Random.Range(-x, x), transform.position.y + 1, pivot.z + Random.Range(-z, z));

        //Debug.Log("Potential spot " + potentialSpot);

        spot = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(potentialSpot, Vector3.down, out hit, 10, raycastLayerMask))
        {
            spot = hit.point;
            spot.y = transform.position.y;

            float distance = Mathf.Infinity;
            for (int i = 0; i < checkpointTransform.childCount; i++)
            {
                float calcDist = (spot - checkpointTransform.GetChild(i).position).sqrMagnitude;

                if (calcDist < distance)
                {
                    distance = calcDist;
                    spotCheckPoint = i;
                }
            }

            CalcNextCheckPoint();
        }
        else
        {
            searchingSpot = false;
            return;
        }

        //Debug.Log("Found spot " + spot);
        foundSpot = true;
        searchingSpot = false;
    }

    private void CalcNextCheckPoint()
    {
        if (spotCheckPoint != checkPoint)
        {
            nextCheckPoint = spotCheckPoint < checkPoint ? checkPoint - 1 : checkPoint + 1;
        }
    }

    private void NextCheckPointSpot()
    {
        Transform nextCheckPointChild = checkpointTransform.GetChild(nextCheckPoint);

        targetPos = Vector3.Lerp(nextCheckPointChild.GetChild(0).position, nextCheckPointChild.GetChild(1).position, Random.Range(0, 1));
    }

    private void LookAtTarget(Vector3 lookPos)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (transform.rotation == targetRotation)
        {
            startWalking = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic)
        {
            return;
        }

        if (grounded && startWalking && !punched)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    private void OnDrawGizmos()
    {
        if (foundSpot)
        {
            Gizmos.DrawWireSphere(spot, 10);
        }

        Gizmos.DrawWireSphere(transform.position, groundedRadius);
    }
}
