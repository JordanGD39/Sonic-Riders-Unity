using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbulenceGenerator : MonoBehaviour
{
    private Dictionary<Transform, TurbulenceRider> turbulencePlayers = new Dictionary<Transform, TurbulenceRider>();

    private PlayerCheckpoints playerCheckpoints;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private RaceManager raceManager;

    private Vector3 prevPos;

    [SerializeField] private float distanceToGeneratePath = 10;
    [SerializeField] private int maxPoints = 80;
    [SerializeField] private float trailTime = 20;
    [SerializeField] private float checkRadius = 10;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private List<Vector3> pathPositions = new List<Vector3>();
    [SerializeField] private List<Vector3> offsetPositions = new List<Vector3>();
    private bool startTrailing = false;
    private bool pausedGeneration = false;

    private float timer = 0;
    private float endTimer = 0;
    private float chanceToStartTurbulence = 40;
    
    [SerializeField] private float offset = 6;
    [SerializeField] private float removingTolerance = 5;
    private TurbulenceObjects turbulenceObjects;

    private TurbulenceRider turbulenceRider;

    private PipeMeshGenerator tubeRenderer;
    private AvailabePipeGenerators pipeGenerators;
    private int pipeIndex = 0;
    private int removePipeIndex = 0;
    private int lapUsed = 0;
    //private bool countAllPoints = false;
    //[SerializeField] private int allVisiblePoints;

    //[SerializeField] private List<TurbulenceObject> placedTurObjects = new List<TurbulenceObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            enabled = false;
            return;
        }

        turbulenceRider = GetComponent<TurbulenceRider>();
        pipeGenerators = FindObjectOfType<AvailabePipeGenerators>();
        raceManager = FindObjectOfType<RaceManager>();
        tubeRenderer = pipeGenerators.PipeMeshGenerators[0];
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCheckpoints = GetComponent<PlayerCheckpoints>();
        //turbulenceObjects = FindObjectOfType<TurbulenceObjects>();

        GameObject[] players = GameObject.FindGameObjectsWithTag(Constants.Tags.player);

        if (raceManager.PlayerPlacing.Count > 1)
        {
            InvokeRepeating("CheckStartTurbulence", 10, 5);
        }

        for (int i = 0; i < players.Length; i++)
        {
            turbulencePlayers.Add(players[i].transform, players[i].GetComponent<TurbulenceRider>());
        }
    }

    public void PauseGeneration()
    {
        pausedGeneration = true;

        if (startTrailing && tubeRenderer.points.Count > 0 && timer < trailTime)
        {
            offsetPositions.Clear();

            if (pipeIndex > pipeGenerators.PipeMeshGenerators.Count - 2)
            {
                timer = trailTime;
                return;
            }

            pipeIndex++;            

            tubeRenderer = pipeGenerators.PipeMeshGenerators[pipeIndex];
            tubeRenderer.points.Clear();            
        }        
    }

    public void ResumeGeneration(Vector3 newPos)
    {
        prevPos = newPos;
        pausedGeneration = false;
    }

    private void CheckStartTurbulence()
    {
        float rand = Random.Range(0, 100);

        float mult = playerMovement.Speed / 66.67f;

        if (mult > 2)
        {
            mult = 2;
        }

        float chance = chanceToStartTurbulence * mult;

        if (playerMovement.Speed > 66.67f)
        {
            Debug.Log("random number: " + rand + " chance: " + chance + " placing: " + playerCheckpoints.Place + " max placing: " + (float)raceManager.PlayerPlacing.Count / 2 + " trail start: " + startTrailing + " already used in lap: " + (lapUsed != playerCheckpoints.LapCount) + " someone making tur: " + raceManager.TurbulenceIsMade);
            Debug.Log(!startTrailing && playerMovement.Speed > 66.67f && rand < chance && playerCheckpoints.Place < (float)raceManager.PlayerPlacing.Count / 2 && lapUsed != playerCheckpoints.LapCount && !raceManager.TurbulenceIsMade);
        }        

        if (!startTrailing && playerMovement.Speed > 66.67 && rand < chance && playerCheckpoints.Place < (float)raceManager.PlayerPlacing.Count / 2 && lapUsed != playerCheckpoints.LapCount && !raceManager.TurbulenceIsMade)
        {
            StartPathGeneration();
        }
    }

    private void Update()
    {
        if (!startTrailing)
        {
            return;
        }

        CheckCollision();

        if (pausedGeneration)
        {
            return;
        }

        if (playerCheckpoints.CurrCheckpoint > 90)
        {
            endTimer += Time.deltaTime;
        }

        if (timer < trailTime)
        {
            timer += Time.deltaTime;
            AddPoint();                
        }
        else if ((DistanceToPoint() || endTimer > 0.25f))
        {
            if (pathPositions.Count == 0)
            {
                startTrailing = false;
                raceManager.TurbulenceIsMade = false;
                timer = 0;

                return;
            }

            prevPos = transform.position;
            RemoveFirstPoint();            
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
                    if (colliders[j].transform.root != transform && colliders[j].transform.root.gameObject.CompareTag(Constants.Tags.player))
                    {
                        Debug.Log("Player in turbulence: " + colliders[j].transform.root.gameObject.name + " in pos: " + i);

                        Transform playerTransform = colliders[j].transform.root;
                        turbulencePlayers[playerTransform].SetTurbulencePoints(pathPositions, offset, i);
                    }
                }
            }
        }
    }

    private void RemoveFirstPoint()
    {
        if (pathPositions.Count == 0)
        {
            return;
        }

        /*if (!countAllPoints)
        {
            allVisiblePoints = 0;

           for (int i = 0; i < pipeIndex + 1; i++)
           {
                allVisiblePoints += pipeGenerators.PipeMeshGenerators[i].points.Count;
           }
        }*/        

        if (pipeGenerators.PipeMeshGenerators[removePipeIndex].points.Count == 0 && removePipeIndex < pipeIndex)
        {
            removePipeIndex++;            
        }

        PipeMeshGenerator currMeshGenerator = pipeGenerators.PipeMeshGenerators[removePipeIndex];        

        if (removePipeIndex == pipeIndex)
        {
            if (offsetPositions.Count > 0)
            {
                offsetPositions.RemoveAt(0);
            }
            
            currMeshGenerator.points = new List<Vector3>(offsetPositions);
            //allVisiblePoints = offsetPositions.Count;
        }
        else
        {
            Vector3 pos = currMeshGenerator.points[0] - CalculateOffset();

            if (pos.magnitude - pathPositions[0].magnitude < removingTolerance)
            {
                currMeshGenerator.points.RemoveAt(0);
            }   
            else
            {
                Debug.Log("Not removing pos: " + (pos.magnitude - pathPositions[0].magnitude));
            }
        }

        pathPositions.RemoveAt(0);       

        CheckBeforeRender(currMeshGenerator);

        endTimer = 0;

        /*for (int i = objectsBetweenPoints; i > 0; i--)
        {
            if (i > placedTurObjects.Count - 1)
            {
                i = placedTurObjects.Count - 1;
            }

            Debug.Log("Removing " + i + " object from turbulence list");

            placedTurObjects[i].FadeOut();
            placedTurObjects.Remove(placedTurObjects[i]);
        }*/
    }

    private void CheckBeforeRender(PipeMeshGenerator currMeshGenerator)
    {
        currMeshGenerator.CurrentMeshRender.enabled = currMeshGenerator.points.Count > 1;

        if (currMeshGenerator.points.Count > 1)
        {
            currMeshGenerator.RenderPipe();
        }
    }

    private bool DistanceToPoint()
    {
        return (prevPos - transform.position).magnitude > distanceToGeneratePath;
    }

    private void AddPoint()
    {
        if (playerMovement.Speed > 0 && DistanceToPoint())
        {
            Vector3 oldPos = prevPos;
            prevPos = transform.position;       

            pathPositions.Add(prevPos);
            AddOffsetPos(prevPos);

            tubeRenderer.points = new List<Vector3>(offsetPositions);

            CheckBeforeRender(tubeRenderer);

            if (pathPositions.Count >= maxPoints)
            {
                timer = trailTime;
            }

            /*for (int i = 0; i < objectsBetweenPoints; i++)
            {
                TurbulenceObject turObject = turbulenceObjects.TurbulenceList[poolIndex];

                float percent = 1 / (float)objectsBetweenPoints;

                turObject.transform.position = Vector3.Lerp(oldPos, prevPos, percent * i);
                turObject.transform.GetChild(0).LookAt(prevPos);
                turObject.transform.up = transform.up;
                turObject.gameObject.SetActive(true);

                //placedTurObjects.Add(turObject);

                poolIndex++;

                if (poolIndex == turbulenceObjects.TurbulenceList.Count)
                {
                    poolIndex = 0;
                }
            }*/

            //vertexPath = GeneratePath(pathPositions.ToArray(), false);
        }
    }

    private Vector3 CalculateOffset()
    {
        return transform.up * offset;
    }

    private void AddOffsetPos(Vector3 pos)
    {
        Vector3 offsetPos = pos + CalculateOffset();
        offsetPositions.Add(offsetPos);
    }

    public void StartPathGeneration()
    {
        if (startTrailing)
        {
            return;
        }

        raceManager.TurbulenceIsMade = true;
        lapUsed = playerCheckpoints.LapCount;

        pipeIndex = 0;
        removePipeIndex = 0;

        tubeRenderer = pipeGenerators.PipeMeshGenerators[0];
        tubeRenderer.points.Clear();
        tubeRenderer.CurrentMeshRender.enabled = true;

        pathPositions.Clear();
        prevPos = transform.position;

        if (!pausedGeneration)
        {
            pathPositions.Add(prevPos);

            AddOffsetPos(prevPos);
        }        

        startTrailing = true;
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
