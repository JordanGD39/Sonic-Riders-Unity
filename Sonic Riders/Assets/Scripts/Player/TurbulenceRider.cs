using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TurbulenceRider : MonoBehaviour
{
    private VertexPath vertexPath;
    public VertexPath Path { set { vertexPath = value; } }
    
    private PlayerGrind playerGrind;
    private CharacterStats charStats;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    [SerializeField] private bool inTurbulence = false;
    public bool InTurbulence { get { return inTurbulence; } }
    private List<Vector3> turbulencePoints = new List<Vector3>();

    [SerializeField] private int targetIndex = 0;
    private Vector3 targetIndexPosition;
    private Vector3 prevPos;
    private Vector3 velocity;
    private float speed = 66;

    public bool InTurbulenceRange { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTurbulence)
        {
            InTurbulenceRange = false;
        }
        else
        {
            /*if (vertexPath != null)
            {
                playerGrind.PathVertex = vertexPath;
            }*/

            CheckValidIndex();

            if (!CheckInTurbulence(targetIndex, false))
            {
                return;
            }

            FollowPath();
        }
    }

    private void FollowPath()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, turbulencePoints[targetIndex], step);
        //Vector3 lookPos = turbulencePoints[targetIndex];
        //lookPos.y = transform.position.y;
        //transform.GetChild(0).LookAt(lookPos);

        Vector3 distance = prevPos - transform.position;
        velocity = distance / Time.deltaTime;

        transform.GetChild(0).forward = -distance.normalized;

        charStats.Hud.UpdateSpeedText(velocity.magnitude);

        prevPos = transform.position;

        if (transform.position == turbulencePoints[targetIndex])
        {
            targetIndex++;

            if (!CheckInTurbulence(targetIndex, true))
            {
                return;
            }

            targetIndexPosition = turbulencePoints[targetIndex];
        }
    }

    private bool CheckInTurbulence(int i, bool changes)
    {
        if (i < 0 || i >= turbulencePoints.Count)
        {
            if (changes)
            {
                OutTurbulence();
            }
            
            return false;
        }

        return true;
    }

    private void CheckValidIndex()
    {
        if (turbulencePoints[targetIndex] != targetIndexPosition)
        {
            targetIndex--;

            if (!CheckInTurbulence(targetIndex, true))
            {
                return;
            }

            targetIndexPosition = turbulencePoints[targetIndex];
        }
    }

    private void OutTurbulence()
    {
        Debug.Log("Fell out");
        inTurbulence = false;
        playerGrind.Grinding = false;
        playerGrind.ChangeRbMode(false);
        playerMovement.Speed = velocity.magnitude;
        rb.velocity = velocity;
    }

    public void SetTurbulencePoints(List<Vector3> points, int indexTouching)
    {
        InTurbulenceRange = true;

        if (inTurbulence)
        {
            return;
        }

        turbulencePoints = points;

        if (!CheckInTurbulence(indexTouching + 1, false))
        {
            return;
        }

        targetIndex = indexTouching + 1;
        targetIndexPosition = turbulencePoints[targetIndex + 1];
    }

    public void CheckTurbulence()
    {
        if (!inTurbulence && !playerMovement.Grounded && InTurbulenceRange)
        {
            inTurbulence = true;

            playerGrind.Grinding = true;
            playerGrind.ChangeRbMode(true);

            if (!CheckInTurbulence(targetIndex - 1, false))
            {
                return;
            }

            transform.position = turbulencePoints[targetIndex - 1];
        }    
        else if(inTurbulence)
        {
            OutTurbulence();
        }
    }
}
