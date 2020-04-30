using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private HUD hud;
    private CharacterStats charStats;
    private BoardStats stats;
    private PlayerBoost playerBoost;
    private PlayerJump playerJump;

    private Rigidbody rb;

    public Vector3 Movement { get; set; }
    public bool Drifting { get; set; } = false;
    public float TurnAmount { get; set; }
    [SerializeField] private float speed = 3;
    public float Speed { get { return speed; } set { speed = value; } }
    private float rotationAmount = 0;
    public float RotationAmount { get { return rotationAmount; } }
    [SerializeField] private bool ridingOnWall = false;
    [SerializeField] private bool fallToTheGround = false;
    public bool FallToTheGround { set { fallToTheGround = value; } }

    [SerializeField] private bool grounded = false;
    public bool Grounded { get { return grounded; } }
    [SerializeField] private float slowdownAngle = 0.4f;
    [SerializeField] private float raycastLength = 0.8f;

    private Vector3 localLandingVelocity = Vector3.zero;

    private float upsideDownTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
        charStats = GetComponent<CharacterStats>();
        stats = transform.GetChild(0).GetChild(1).GetComponent<BoardStats>();
        playerBoost = GetComponent<PlayerBoost>();
        playerJump = GetComponent<PlayerJump>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        grounded = GetAlignment();

        if (grounded)
        {
            if (!ridingOnWall)
            {
                fallToTheGround = false;
            }
            rotationAmount = TurnAmount * stats.Cornering;

            rotationAmount *= Time.deltaTime;            
            transform.GetChild(0).Rotate(0, rotationAmount, 0);
        }

        Acceleration();

        if (charStats.Air > 0 && Movement.z != 0 && speed > 0 && grounded)
        {
            charStats.Air -= stats.AirDepletion;
        }
    }

    private bool GetAlignment()
    {
        bool onGround = false;

        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, -0.03f, 0), -transform.up, Color.red);

        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(transform.position + new Vector3(0, -0.03f, 0), -transform.up, out hit, raycastLength, layerMask))
        {
            if (!hit.collider.isTrigger)
            {
                transform.up -= (transform.up - hit.normal) * 0.1f;
                onGround = true;

                if (!grounded)
                {
                    float normalCalc = -(hit.normal.y - 1);

                    if (normalCalc > 1)
                    {
                        normalCalc = 1;
                    }

                    float tempSpeed = speed * normalCalc;

                    if (tempSpeed < 5)
                    {
                        tempSpeed = localLandingVelocity.z;
                    }

                    Debug.Log(tempSpeed + " up calc: " + normalCalc);

                    speed = tempSpeed;
                }
            }
        }
        else
        {           
            transform.rotation = Quaternion.RotateTowards(transform.rotation, new Quaternion(0, 0, 0, transform.rotation.w), 10);

            if (transform.rotation.x == 1)
            {
                upsideDownTimer += Time.deltaTime;

                if (upsideDownTimer > 0.5f)
                {
                    transform.rotation = new Quaternion(0, 0, transform.rotation.z, transform.rotation.w);
                    upsideDownTimer = 0;
                }
            }
        }

        return onGround;
    }

    private void FixedUpdate()
    {
        Vector3 localVel = transform.GetChild(0).forward * speed;

        if (transform.rotation.x > -slowdownAngle && transform.rotation.x < slowdownAngle || !grounded)
        {            
            localVel.y = rb.velocity.y;
            ridingOnWall = false;
        }      
        else
        {
            ridingOnWall = true;
        }

        if (ridingOnWall && speed < 10 || playerJump.JumpHold && ridingOnWall)
        {
            localVel.y = rb.velocity.y;
            fallToTheGround = true;
        }

        if (grounded && !(fallToTheGround && transform.GetChild(0).forward.y > 0))
        {            
            rb.velocity = localVel;
        }
    }

    private void Acceleration()
    {
        if (grounded)
        {
            if (playerJump.JumpHold)
            {
                if (speed > 5)
                {
                    speed -= 0.2f;
                }

                hud.UpdateSpeedText(speed);

                return;
            }

            //Losing speed when going up walls
            if(!(!ridingOnWall && transform.rotation.x > -slowdownAngle && transform.rotation.x < slowdownAngle || ridingOnWall && rb.velocity.y < 0))
            {
                if (speed > 0)
                {
                    speed -= (stats.Dash + 3f) * Time.deltaTime;
                }
                else if(speed < 0)
                {
                    speed += (stats.Dash + 3f) * Time.deltaTime;
                }

                hud.UpdateSpeedText(speed);
            }

            if (fallToTheGround)
            {
                if (transform.GetChild(0).forward.y > 0)
                {
                    speed = 0;
                }

                hud.UpdateSpeedText(rb.velocity.magnitude);
            }
            else
            {
                hud.UpdateSpeedText(speed);
            }

            if (Drifting)
            {
                return;
            }

            if (Movement.z >= 0.25f)
            {
                if (speed < stats.Limit[charStats.Level])
                {
                    speed += stats.Dash * Movement.z * Time.deltaTime;                                            
                }
                else
                {
                    if (!ridingOnWall && !playerBoost.Boosting || playerBoost.Boosting && speed > stats.Boost[charStats.Level]&& !ridingOnWall)
                    {
                        speed -= 7 * Time.deltaTime;
                    }     
                    else
                    {
                        if (speed < 100)
                        {
                            speed += 7 * Time.deltaTime;
                        }                                           
                    }
                }                
            }
            else if (Movement.z < 0.25f && Movement.z >= 0)
            {
                if (speed > 0)
                {                    
                    speed -= stats.Dash * (1 + Movement.z) * Time.deltaTime;
                }
                else if (speed < -1)
                {
                    speed += 7 * Time.deltaTime;
                }
                else
                {
                    speed = 0;
                }
            }
            else
            {
                if (speed > -10 && grounded)
                {                    
                    speed -= stats.Dash * (1 + Mathf.Abs(Movement.z)) * Time.deltaTime;
                }
            }                      
        }
        else
        {
            localLandingVelocity = transform.GetChild(0).InverseTransformDirection(rb.velocity);

            speed = localLandingVelocity.magnitude;

            hud.UpdateSpeedText(localLandingVelocity.magnitude);
        }               
    }
}
