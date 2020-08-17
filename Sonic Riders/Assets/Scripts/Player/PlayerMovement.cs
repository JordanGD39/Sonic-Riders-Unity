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
    private PlayerDrift playerDrift;
    private PlayerTricks playerTricks;
    private PlayerFlight playerFlight;
    private ThirdPersonCamera thirdPersonCamera;

    private Rigidbody rb;

    public Vector3 Movement { get; set; }
    public bool Drifting { get; set; } = false;
    public float TurnAmount { get; set; }
    [SerializeField] private float speed = 3;
    public float Speed { get { return speed; } set { speed = value; } }
    [SerializeField] private float rotationAmount = 0;
    public float RotationAmount { get { return rotationAmount; } }
    [SerializeField] private bool ridingOnWall = false;
    [SerializeField] private bool fallToTheGround = false;
    public bool FallToTheGround { set { fallToTheGround = value; } }

    [SerializeField] private bool grounded = false;
    public bool Grounded { get { return grounded; } }
    [SerializeField] private float slowdownAngle = 0.4f;
    [SerializeField] private float raycastLength = 0.8f;
    public float RaycastLength { get { return raycastLength; } set { raycastLength = value; } }
    [SerializeField] private float startingRaycastLength = 0.8f;
    public float StartingRaycastLength { get { return startingRaycastLength; } }
    [SerializeField] private float extraForceGrounded = 500;
    [SerializeField] private float failVelocityMultiplier = 0.3f;

    private Vector3 localLandingVelocity = Vector3.zero;
    private Vector3 lastGroundedPos;
    public Vector3 LastGroundedPos { get { return lastGroundedPos; } }

    private float upsideDownTimer = 0;

    public bool DriftBoost { get; set; } = false;

    public bool CantMove { get; set; } = false;
    public Vector3 GrindVelocity { get; set; }
    public bool IsPlayer { get; set; } = false;
    public bool Bouncing { get; set; } = false;

    [SerializeField] private float hitAngle;
    [SerializeField] private float highestFallSpeed;

    [SerializeField] private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();        
        playerBoost = GetComponent<PlayerBoost>();
        playerJump = GetComponent<PlayerJump>();
        playerDrift = GetComponent<PlayerDrift>();
        playerTricks = GetComponent<PlayerTricks>();
        playerFlight = GetComponent<PlayerFlight>();
        thirdPersonCamera = Camera.main.GetComponentInParent<ThirdPersonCamera>();
    }

    public void CheckIfPlayer()
    {
        charStats = GetComponent<CharacterStats>();
        charStats.IsPlayer = IsPlayer;
        stats = charStats.BoardStats;
    }

    // Update is called once per frame
    void Update()
    {
        if (charStats == null)
        {
            return;
        }

        //Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (!playerJump.DontDragDown)
        {
            grounded = GetAlignment();
        }
        else
        {
            grounded = false;
        }
        

        if (CantMove)
        {
            return;
        }

        if (grounded)
        {
            if (IsPlayer)
            {
                thirdPersonCamera.enabled = false;
                thirdPersonCamera.transform.localRotation = new Quaternion(0, 0, 0, thirdPersonCamera.transform.localRotation.w);
            }

            lastGroundedPos = transform.position;
            transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);

            if (!ridingOnWall)
            {
                fallToTheGround = false;
            }
            rotationAmount = TurnAmount * stats.Cornering;

            rotationAmount *= Time.deltaTime;           

            transform.GetChild(0).Rotate(0, rotationAmount, 0);

            if (DriftBoost)
            {
                for (int i = 0; i < 10; i++)
                {
                    transform.GetChild(0).Rotate(0, TurnAmount * 0.2f, 0);
                }
                
                DriftBoost = false;
            }

            if (playerDrift.DriftPressed)
            {
                transform.GetChild(0).GetChild(0).localRotation = new Quaternion(0, TurnAmount * 0.1f, 0, transform.GetChild(0).GetChild(0).localRotation.w);
            }  
            else
            {
                transform.GetChild(0).GetChild(0).localRotation = new Quaternion(0, 0, 0, 0);
            }
        }
        else
        {
            if (IsPlayer && !playerTricks.CanDoTricks && !playerFlight.Flying)
            {
                if (!thirdPersonCamera.enabled)
                {
                    thirdPersonCamera.ResetRotation();
                    thirdPersonCamera.enabled = true;
                }               
            }

            if (IsPlayer && (playerFlight.Flying || playerTricks.CanDoTricks) && thirdPersonCamera.enabled)
            {
                thirdPersonCamera.enabled = false;
                thirdPersonCamera.transform.localRotation = new Quaternion(0, 0, 0, thirdPersonCamera.transform.localRotation.w);
            }

            raycastLength = 0;
        }

        if (!playerFlight.Flying)
        {
            Acceleration();
        }

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
        
        if (Physics.Raycast(transform.position + new Vector3(0, -0.03f, 0), -transform.GetChild(0).up, out hit, raycastLength, layerMask))
        {
            if (!hit.collider.isTrigger)
            {
                onGround = true;

                hitAngle = Vector3.Angle(Vector3.up, hit.normal);

                if (grounded)
                {
                    transform.up -= (transform.up - hit.normal) * 0.1f;                    
                }                
                else
                {
                    transform.up = hit.normal;

                    float normalCalc = -(hit.normal.y - 1);

                    if (transform.GetChild(0).forward.y > 0)
                    {
                        normalCalc = -normalCalc;
                    }

                    if (normalCalc > 0.1f)
                    {
                        normalCalc += 0.2f;
                    }

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

                    if (playerTricks.CanDoTricks)
                    {
                        playerTricks.Landed();
                    }                    
                }
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, new Quaternion(0, 0, 0, transform.rotation.w), 10);

            if (transform.rotation.x == 1 || transform.rotation.z == 1)
            {
                upsideDownTimer += Time.deltaTime;

                if (upsideDownTimer > 0.5f)
                {
                    transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
                    upsideDownTimer = 0;
                }
            }
        }

        return onGround;
    }

    private void FixedUpdate()
    {
        if (charStats == null || playerFlight.Flying)
        {
            return;
        }

        Vector3 localVel = transform.GetChild(0).forward * speed;

        if (grounded && !playerJump.DontDragDown && !playerTricks.CanDoTricks && raycastLength == startingRaycastLength && !fallToTheGround)
        {
            rb.AddForce(-transform.up * extraForceGrounded);
        }

        if (playerJump.DontDragDown && transform.InverseTransformDirection(rb.velocity).y < 0)
        {
            Debug.Log("There!");
            if (playerJump.RampPower >  0)
            {
                rb.velocity = playerJump.CurrRamp.transform.GetChild(0).forward * (playerJump.JumpHeight + playerJump.RampPower);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, playerJump.JumpHeight * failVelocityMultiplier, rb.velocity.z);
            }
        }

        if (NotOnSlowdownAngle() || !grounded)
        {           
            ridingOnWall = false;
        }      
        else
        {
            ridingOnWall = true;
        }

        if (ridingOnWall && speed < 10)
        {
            localVel.y = rb.velocity.y;
            fallToTheGround = true;            
        }

        if (playerJump.JumpHold)
        {
            localVel.y = rb.velocity.y;
        }

        if (grounded && !(fallToTheGround && transform.GetChild(0).forward.y > -0.8f) && !CantMove)
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

                if (IsPlayer)
                {
                    hud.UpdateSpeedText(speed);
                }

                return;
            }

            //Losing speed when going up walls
            if(!(!ridingOnWall && NotOnSlowdownAngle() || ridingOnWall && rb.velocity.y < 0))
            {
                if (speed > 0)
                {
                    speed -= 13 * Time.deltaTime;
                }
                else if(speed < 0)
                {
                    speed += 13 * Time.deltaTime;
                }

                if (IsPlayer)
                {
                    hud.UpdateSpeedText(speed);
                }
            }

            if (fallToTheGround)
            {
                speed = transform.GetChild(0).InverseTransformDirection(rb.velocity).z;
                //Debug.Log(speed);

                if (IsPlayer)
                {
                    hud.UpdateSpeedText(rb.velocity.magnitude);
                }
            }
            else
            {
                if (IsPlayer)
                {
                    hud.UpdateSpeedText(speed);
                }
            }

            if (Drifting)
            {
                return;
            }

            float brakeMultiplier = 1;

            if (ridingOnWall && transform.GetChild(0).forward.y < -0.5f)
            {
                brakeMultiplier = 0.1f;
            }

            if (Movement.z >= 0.25f)
            {
                if (speed < charStats.GetCurrentLimit())
                {
                    speed += stats.Dash * Movement.z * Time.deltaTime;                                            
                }
                else
                {
                    if ((!playerBoost.Boosting || playerBoost.Boosting && speed > stats.Boost[charStats.Level]) && !ridingOnWall)
                    {
                        speed -= 7 * Time.deltaTime;
                    }     
                    else
                    {
                        if (speed < 100)
                        {
                            if (transform.GetChild(0).forward.y < -0.5f)
                            {
                                speed += 7 * Time.deltaTime;
                            }
                            else if(speed > charStats.GetCurrentLimit())
                            {
                                speed -= 7 * Time.deltaTime;
                            }                            
                        }
                    }
                }                
            }
            else if (Movement.z < 0.25f && Movement.z >= 0)
            {
                if (speed > 0)
                {
                    if (rb.velocity.y < 0 && brakeMultiplier < 1)
                    {
                        if (speed < 100)
                        {
                            speed += 3 * Time.deltaTime;
                        }
                    }
                    else
                    {
                        speed -= (stats.Dash * (1 + Movement.z) * brakeMultiplier) * Time.deltaTime;
                    }                    
                }
                else if (speed < -1)
                {
                    speed += 7 * brakeMultiplier * Time.deltaTime;
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
                    speed -= (stats.Dash * (1 + Mathf.Abs(Movement.z)) * brakeMultiplier) * Time.deltaTime;
                }
            }                      
        }
        else
        {
            localLandingVelocity = transform.GetChild(0).InverseTransformDirection(rb.velocity);

            if (rb.velocity.y < highestFallSpeed)
            {
                highestFallSpeed = rb.velocity.y;
            }            

            speed = localLandingVelocity.magnitude;

            if (IsPlayer)
            {
                hud.UpdateSpeedText(localLandingVelocity.magnitude);
            }
        }               
    }

    private bool NotOnSlowdownAngle()
    {
        if (hitAngle > slowdownAngle)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!playerJump.DontDragDown && !grounded && !Bouncing)
        {
            highestFallSpeed = 0;
            raycastLength = startingRaycastLength;
        }        
    }
}
