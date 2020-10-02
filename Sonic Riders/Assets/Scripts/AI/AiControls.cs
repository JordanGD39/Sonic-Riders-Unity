using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControls : MonoBehaviour
{
    private PlayerMovement playerMovement;
    //private PlayerBoost playerBoost;
    private PlayerDrift playerDrift;

    //AI wont jump only on ramps

    //private PlayerGrind playerGrind;

    private List<Transform> waypoints = new List<Transform>();

    [SerializeField] private int currWaypoint = 0;

    [SerializeField] private Transform rotateMarker;
    [SerializeField] private float distanceTreshold = 5;

    private float prevRot;

    private bool dontRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        //playerBoost = GetComponent<PlayerBoost>();
        playerDrift = GetComponent<PlayerDrift>();
        //playerGrind = GetComponent<PlayerGrind>();

        Transform waypointTransform = GameObject.FindGameObjectWithTag("Waypoints").transform;

        for (int i = 0; i < waypointTransform.childCount; i++)
        {
            waypoints.Add(waypointTransform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.Movement = new Vector3(0, 0, 1);

        Vector3 pos = waypoints[currWaypoint].position;
        pos.y = 0;

        rotateMarker.LookAt(pos);

        Quaternion rot = rotateMarker.localRotation;

        rot.y = Mathf.Clamp(rot.y, -0.99f, 0.99f);

        //Debug.Log(rot.y);

        if (prevRot > 0.35f && rot.y < 0 || prevRot < -0.35f && rot.y > 0)
        {
            rot.y = prevRot;
        }

        prevRot = rot.y;

        rotateMarker.localRotation = rot;

        float horAiInput = rotateMarker.localRotation.y;

        if (horAiInput > 0.5f)
        {
            horAiInput = 0.5f;
        }
        else if (horAiInput < -0.5f)
        {
            horAiInput = -0.5f;
        }

        horAiInput *= 2;        

        //Debug.Log(horAiInput);

        float distance = (waypoints[currWaypoint].position - transform.position).sqrMagnitude;

        if (distance  < distanceTreshold)
        {
            if (waypoints[currWaypoint].GetComponent<WaypointAction>() != null)
            {
                if (waypoints[currWaypoint].GetComponent<WaypointAction>().Looping)
                {
                    dontRotate = true;
                }
                else if(!waypoints[currWaypoint].GetComponent<WaypointAction>().Looping && dontRotate)
                {
                    dontRotate = false;
                }
            }

            currWaypoint++;

            if (currWaypoint > waypoints.Count - 1)
            {
                currWaypoint = 0;
            }
        }

        //float turnDir = Input.GetAxis("Horizontal") + playerDrift.DriftDir;

        //if (playerDrift.DriftPressed && playerMovement.Grounded)
        //{
        //    if (Mathf.Abs(turnDir) < 0.2f)
        //    {
        //        turnDir = 0.2f * playerDrift.DriftDir;
        //    }
        //    else if (Mathf.Abs(turnDir) > 1.5f)
        //    {
        //        turnDir = 1.5f * playerDrift.DriftDir;
        //    }
        //}

        if (!dontRotate)
        {
            playerMovement.TurnAmount = horAiInput;
        }      
        else
        {
            playerMovement.TurnAmount = 0;
        }

        //playerBoost.BoostPressed = Input.GetButtonDown("Boost");

        //playerDrift.DriftPressed = Input.GetButton("Drift");

        //playerGrind.JumpPressed = Input.GetButtonDown("Jump");
    }
}
