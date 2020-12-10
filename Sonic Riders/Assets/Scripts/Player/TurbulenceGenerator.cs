using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TurbulenceGenerator : MonoBehaviour
{
    private Dictionary<Transform, TurbulenceRider> turbulencePlayers = new Dictionary<Transform, TurbulenceRider>();

    private VertexPath vertexPath;
    public VertexPath GetVertexPath { get { return vertexPath; } }

    private PlayerCheckpoints playerCheckpoints;
    private Rigidbody rb;

    private Vector3 prevPos;
    [SerializeField] private float distanceToGeneratePath = 10;
    [SerializeField] private float maxPoints = 20;
    [SerializeField] private float trailTime = 10;
    [SerializeField] private float checkRadius = 5;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private List<Vector3> pathPositions = new List<Vector3>();
    private bool startTrailing = false;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        playerCheckpoints = GetComponent<PlayerCheckpoints>();

        GameObject[] players = GameObject.FindGameObjectsWithTag(Constants.Tags.player);

        for (int i = 0; i < players.Length; i++)
        {
            turbulencePlayers.Add(players[i].transform, players[i].GetComponent<TurbulenceRider>());
        }
    }

    private void Update()
    {
        if (startTrailing && pathPositions.Count == 0)
        {
            startTrailing = false;
            timer = 0;
        }

        if (startTrailing)
        {
            if (timer < trailTime)
            {
                timer += Time.deltaTime;
                AddPoint();
                CheckCollision();
            }
            else if (DistanceToPoint() && pathPositions.Count > 0)
            {
                pathPositions.RemoveAt(0);
            }
        }        
    }

    private void CheckCollision()
    {
        for (int i = 0; i < pathPositions.Count - 1; i++)
        {
            Collider[] colliders = Physics.OverlapCapsule(pathPositions[i], pathPositions[i + 1], checkRadius, layerMask);

            if (colliders.Length > 0)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (colliders[j].transform.root != transform)
                    {
                        Debug.Log("Player in turbulence: " + colliders[j].transform.root.gameObject.name + " in pos: " + i);

                        turbulencePlayers[colliders[j].transform.root].Path = vertexPath;
                    }
                }
            }
        }
    }

    private bool DistanceToPoint()
    {
        return (prevPos - transform.position).magnitude > distanceToGeneratePath;
    }

    private void AddPoint()
    {
        if (DistanceToPoint())
        {
            prevPos = transform.position;
            pathPositions.Add(prevPos);

            if (pathPositions.Count > maxPoints)
            {
                pathPositions.RemoveAt(0);
            }

            vertexPath = GeneratePath(pathPositions.ToArray(), false);
        }
    }

    public void StartPathGeneration()
    {
        if (startTrailing)
        {
            return;
        }

        pathPositions.Clear();
        prevPos = transform.position;
        pathPositions.Add(prevPos);
        startTrailing = true;
    }

    private VertexPath GeneratePath(Vector3[] points, bool closedPath)
    {
        BezierPath bezierPath = new BezierPath(points, closedPath, PathSpace.xyz);

        return new VertexPath(bezierPath, transform);
    }

    private void OnDrawGizmos()
    {
        if (pathPositions.Count > 1)
        {
            for (int i = 0; i < pathPositions.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPositions[i], pathPositions[i + 1]);
                Gizmos.DrawWireSphere(pathPositions[i], checkRadius);
            }
        }
    }
}
