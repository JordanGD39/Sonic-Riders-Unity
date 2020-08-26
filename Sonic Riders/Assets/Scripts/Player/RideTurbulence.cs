using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class RideTurbulence : MonoBehaviour
{
    private ParticleSystem ps;

    private VertexPath path;

    [SerializeField] private LayerMask layerMask;

    private TurbulenceDetect turbulence;
    private Transform player;
    private Rigidbody playerRb;
    private PlayerMovement playerMovement;

    [SerializeField] private int currIndex = 0;

    private bool readyToGo = false;
    [SerializeField] private float speed = 40;
    [SerializeField] private float closestDistance = 0;

    private List<Vector3> positions = new List<Vector3>();

    public void ReadyToStart(ParticleSystem.Particle aParticle, TurbulenceDetect turbulenceDetect, Transform aPlayer, ParticleSystem aPs)
    {
        ps = aPs;
        player = aPlayer;
        playerRb = player.GetComponent<Rigidbody>();
        playerRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        playerRb.isKinematic = true;
        playerMovement = player.GetComponent<PlayerMovement>();
        turbulence = turbulenceDetect;

        GetParticles();

        closestDistance = path.GetClosestDistanceAlongPath(transform.position);
        transform.rotation = Quaternion.LookRotation(path.GetDirectionAtDistance(closestDistance, EndOfPathInstruction.Stop));
        transform.position = path.GetClosestPointOnPath(transform.position);

        readyToGo = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!readyToGo)
        {
            return;
        }

        GetParticles();

        closestDistance += speed * Time.deltaTime;
        Vector3 desiredPos = path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
        transform.position = desiredPos;
        transform.rotation = path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);

        player.transform.localPosition = new Vector3(0, 0, 0);

        /*float step = speed * Time.deltaTime;

        if (IndexInRange())
        {
            Debug.Log(currIndex - 1);
            Vector3 nextPos = particles[currIndex - 1].position + player.TransformDirection(0, 4, 0);

            transform.position = Vector3.MoveTowards(transform.position, nextPos, step);
            transform.LookAt(nextPos);

            if ((transform.position - nextPos).sqrMagnitude < distance && IndexInRange())
            {
                particle = particles[currIndex - 1];
            }
        }
        else
        {
            player.parent = null;
            playerMovement.CantMove = false;
            playerRb.isKinematic = false;
            playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            readyToGo = false;
            
            Invoke("ReinsertPlayer", 5);
        }*/
    }

    /*private bool IndexInRange()
    {
        return particles.Count > 0 && currIndex - 1 >= 0 && currIndex - 1 < particles.Count;
    }*/

    private void ReinsertPlayer()
    {
        turbulence.PlayerColliders.Add(player.GetComponentInChildren<PlayerTrigger>().GetComponent<CapsuleCollider>());
        Destroy(gameObject);
    }

    private void GetParticles()
    {
        positions.Clear();
        List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
        ParticleSystem.Particle[] array = new ParticleSystem.Particle[ps.main.maxParticles];

        ps.GetParticles(array);        
        particles.AddRange(array);
        particles.Sort(SortByLifeTime);

        for (int i = 0; i < particles.Count; i++)
        {
            positions.Add(particles[i].position);
        }

        path = GeneratePath(positions.ToArray(), false);
    }


    VertexPath GeneratePath(Vector3[] points, bool closedPath)
    {
        // Create a closed, 3D bezier path from the supplied points array
        // These points are treated as anchors, which the path will pass through
        // The control points for the path will be generated automatically
        BezierPath bezierPath = new BezierPath(points, closedPath, PathSpace.xyz);
        // Then create a vertex path from the bezier path, to be used for movement etc
        return new VertexPath(bezierPath, ps.transform, 0.01f);
    }


    static int SortByLifeTime(ParticleSystem.Particle p1, ParticleSystem.Particle p2)
    {
        return p1.remainingLifetime.CompareTo(p2.remainingLifetime);
    }
}
