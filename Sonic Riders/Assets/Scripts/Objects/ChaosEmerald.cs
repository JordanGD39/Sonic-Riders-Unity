using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ChaosEmerald : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    [SerializeField] private Transform model;
    
    public bool CanCatch { get; set; } = true;

    private Transform raceCheckpointsParent;
    [SerializeField] private float spinSpeed = 15;

    private Vector3 posToFlyTo;
    [SerializeField] private float speed = 10;
    [SerializeField] private float pathSpeed = 10;

    private Vector3 checkpointForward;
    private Transform survivalParent;

    [SerializeField] private int checkPoint = 2;
    public int Checkpoint { get { return checkPoint; } }

    private void Start()
    {
        survivalParent = transform.parent;
        raceCheckpointsParent = GameObject.FindGameObjectWithTag(Constants.Tags.raceManager).transform;
    }

    // Update is called once per frame
    void Update()
    {
        model.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && other.transform.parent == raceCheckpointsParent)
        {
            checkPoint = other.transform.GetSiblingIndex();
        }
    }

    public void FlyToPos(Vector3 pos, Vector3 forward)
    {
        CanCatch = false;
        posToFlyTo = pos;
        checkpointForward = forward;

        StartCoroutine("FlyingToPos");
    }

    private IEnumerator FlyingToPos()
    {
        Vector3 closestPointPath = path.path.GetClosestPointOnPath(transform.position);
        closestPointPath.y = 1.5f;

        while (closestPointPath != transform.position)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, closestPointPath, step);
            transform.LookAt(new Vector3(closestPointPath.x, transform.position.y, closestPointPath.z));
            yield return null;
        }

        float closestDistance = path.path.GetClosestDistanceAlongPath(transform.position);

        while ((posToFlyTo - transform.position).sqrMagnitude > 1000)
        {
            float step = pathSpeed * Time.deltaTime;

            closestDistance += step;
            Vector3 desiredPos = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Loop);
            desiredPos.y = transform.position.y;
            transform.position = desiredPos;

            int extraLook = 1;

            if (pathSpeed < 0)
            {
                extraLook = -1;
            }

            Vector3 lookDir = path.path.GetPointAtDistance(closestDistance + extraLook, EndOfPathInstruction.Loop);
            lookDir.y = transform.position.y;
            transform.LookAt(lookDir);

            yield return null;
        }

        CanCatch = true;

        while (posToFlyTo != transform.position)
        {
            if (transform.parent != survivalParent)
            {
                break;
            }

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, posToFlyTo, step);
            transform.LookAt(new Vector3(posToFlyTo.x, transform.position.y, posToFlyTo.z));
            yield return null;
        }

        transform.forward = checkpointForward;
    }
}
