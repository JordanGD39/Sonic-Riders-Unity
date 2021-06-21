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
    private PlayerJump playerJump;

    [SerializeField] private bool inTurbulence = false;
    public bool InTurbulence { get { return inTurbulence; } }
    private List<Vector3> turbulencePoints = new List<Vector3>();

    [SerializeField] private int targetIndex = 0;
    private Vector3 targetIndexPosition;
    private Vector3 prevPos;
    private Vector3 velocity;
    private float pointOffset;
    private float speed = 70;
    [SerializeField] private float rotateSpeed = 310;
    [SerializeField] private float rotateSpeedOnHalfPipe = 180;
    [SerializeField] private float airGain = 21;

    private bool firstRot = false;
    private int distanceZeroCount = 0;

    public bool InTurbulenceRange { get; set; } = false;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        playerGrind = GetComponent<PlayerGrind>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTurbulence)
        {
            timer += Time.deltaTime;

            if (timer > 0.2f)
            {
                InTurbulenceRange = false;
            }            
        }
        else
        {
            /*if (vertexPath != null)
            {
                playerGrind.PathVertex = vertexPath;
            }*/

            CheckValidIndex();

            if (!CheckInTurbulence(targetIndex, false, -1))
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

        if (distance.magnitude < 1)
        {
            distanceZeroCount++;

            if (distanceZeroCount > 3)
            {
                OutTurbulence();
            }
        }
        else
        {
            distanceZeroCount = 0;
        }

        velocity = distance / Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-distance.normalized), rotateSpeed * Time.deltaTime);

        if (!firstRot)
        {
            transform.GetChild(0).forward = transform.forward;
            firstRot = true;
        }

        transform.GetChild(0).RotateAround(transform.position + transform.up * pointOffset, transform.forward, playerMovement.Movement.x * rotateSpeedOnHalfPipe * Time.deltaTime);

        Quaternion localRot = transform.GetChild(0).localRotation;
        localRot.z = Mathf.Clamp(localRot.z, -0.6f, 0.6f);
        transform.GetChild(0).localRotation = localRot;

        Vector3 localPos = transform.GetChild(0).localPosition;
        localPos.y = Mathf.Clamp(localPos.y, 0, pointOffset);
        transform.GetChild(0).localPosition = localPos;

        charStats.Air += charStats.BoardStats.RingsAsAir ? 2 * Time.deltaTime : airGain * Time.deltaTime;

        if (charStats.Cam != null)
        {
            charStats.Cam.transform.forward = transform.forward;
        }

        charStats.Hud.UpdateSpeedText(speed);

        prevPos = transform.position;

        if (transform.position == turbulencePoints[targetIndex])
        {
            targetIndex++;

            if (!CheckInTurbulence(targetIndex, true, targetIndex - 1))
            {
                return;
            }

            targetIndexPosition = turbulencePoints[targetIndex];
        }
    }

    private bool CheckInTurbulence(int i, bool changes, int prevI)
    {
        bool indexTooFar = i < 0 || i > turbulencePoints.Count - 1;

        if (indexTooFar || (prevI > 0 && !indexTooFar && (turbulencePoints[prevI] - turbulencePoints[i]).sqrMagnitude > 500))
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
            if (!CheckInTurbulence(targetIndex - 1, true, targetIndex))
            {
                return;
            }

            targetIndex--;

            targetIndexPosition = turbulencePoints[targetIndex];
        }
    }

    public void OutTurbulence()
    {
        Debug.Log("Fell out");
        
        transform.position = transform.GetChild(0).position;
        transform.GetChild(0).localPosition = Vector3.zero;
        Vector3 oldForward = transform.forward;
        Quaternion localRot = transform.localRotation;
        localRot.y = 0;
        transform.localRotation = localRot;
        transform.GetChild(0).forward = oldForward;
        inTurbulence = false;
        playerGrind.Grinding = false;
        playerGrind.ChangeRbMode(false);
        playerMovement.AboveSea(false);
        playerMovement.Speed = speed;
        rb.velocity = transform.GetChild(0).forward * speed;

        playerJump.OffTurbulence();
    }

    public void SetTurbulencePoints(List<Vector3> points, float offset, int indexTouching)
    {
        timer = 0;
        InTurbulenceRange = true;

        if (inTurbulence || playerMovement.Attacked || playerMovement.UnderWater)
        {
            return;
        }

        turbulencePoints = points;
        pointOffset = offset;

        if (!CheckInTurbulence(indexTouching + 1, false, -1))
        {
            return;
        }

        targetIndex = indexTouching + 1;
        targetIndexPosition = turbulencePoints[targetIndex];
    }

    public void CheckTurbulence()
    {
        if (!enabled)
        {
            return;
        }

        if (!inTurbulence && !playerMovement.Grounded && InTurbulenceRange)
        {
            firstRot = false;

            inTurbulence = true;

            playerGrind.Grinding = true;
            playerGrind.ChangeRbMode(true);

            if (!CheckInTurbulence(targetIndex - 1, false, -1))
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
