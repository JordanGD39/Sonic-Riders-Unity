﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerGrind : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump playerJump;
    private Rigidbody rb;
    private BoardStats stats;
    private CharacterStats charStats;

    public bool JumpPressed { get; set; } = false;

    [SerializeField] private PathCreator path;
    public PathCreator Path { get { return path; } set { path = value; } }
    public Quaternion StartingRotation { get; set; }

    [SerializeField] private bool grinding = false;
    public bool Grinding { get { return grinding; } }

    private float closestDistance = 0;

    [SerializeField] private float speed = 3;
    [SerializeField] private float extraCharHeight = 0.2f;
    [SerializeField] private float jumpHeightOfRamp = 30;

    private Vector3 previousPos;
    [SerializeField] private Vector3 velocity;

    private HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        rb = GetComponent<Rigidbody>();
        charStats = GetComponent<CharacterStats>();
        stats = movement.Stats;
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Path != null)
        {
            if (grinding)
            {
                velocity = (transform.position - previousPos) / Time.deltaTime;
                previousPos = transform.position;

                if (velocity.y > 0)
                {
                    if (movement.Speed > 0)
                    {
                        movement.Speed -= (stats.Dash - 3f) * Time.deltaTime;
                    }
                    else
                    {
                        movement.Speed += (stats.Dash - 3f) * Time.deltaTime;
                    }
                    
                }
                else if(velocity.y < 0)
                {
                    if (movement.Speed > 0)
                    {
                        movement.Speed += (stats.Dash - 3f) * Time.deltaTime;
                    }
                    else
                    {
                        movement.Speed -= (stats.Dash - 3f) * Time.deltaTime;
                    }
                }

                if (movement.Speed <= 1 && movement.Speed >= 0)
                {
                    movement.Speed = -1;
                }

                speed = movement.Speed;

                hud.UpdateSpeedText(speed);

                closestDistance += speed * Time.deltaTime;
                Vector3 desiredPos = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
                desiredPos.y += extraCharHeight;
                transform.position = desiredPos;
                transform.GetChild(0).localRotation = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);

                if (JumpPressed || path.path.GetClosestTimeOnPath(transform.position) > 0.99f)
                {
                    bool jumpPressed = JumpPressed;
                    grinding = false;
                    transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
                    rb.isKinematic = false;
                    Vector3.ClampMagnitude(velocity, stats.Boost[charStats.Level]);
                    rb.velocity = velocity;
                    movement.CantMove = false;

                    if (jumpPressed)
                    {
                        playerJump.JumpHeight = jumpHeightOfRamp;
                        playerJump.JumpRelease = true;
                    }                    
                }

                return;
            }

            if (!movement.Grounded && JumpPressed && !grinding)
            {
                rb.isKinematic = true;
                movement.CantMove = true;
                speed = movement.Speed;
                closestDistance = path.path.GetClosestDistanceAlongPath(transform.position);
                transform.GetChild(0).localRotation = Quaternion.LookRotation(path.path.GetDirectionAtDistance(closestDistance, EndOfPathInstruction.Stop));
                transform.position = path.path.GetClosestPointOnPath(transform.position);                
                grinding = true;
            }            
        }        
    }
}
