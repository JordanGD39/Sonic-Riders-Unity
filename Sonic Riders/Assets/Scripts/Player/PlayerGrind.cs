using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerGrind : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump playerJump;
    private PlayerSound playerSound;
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
    [SerializeField] private float jumpSpeed = 20;
    [SerializeField] private float airGain = 0.02f;
    [SerializeField] private float extraCharHeight = 0.2f;
    [SerializeField] private float jumpHeightOfRail = 30;

    private Vector3 previousPos;
    [SerializeField] private Vector3 velocity;

    private HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();

        if (!charStats.TypeCheck(type.SPEED))
        {
            enabled = false;
        }

        movement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        rb = GetComponent<Rigidbody>();
        stats = charStats.BoardStats;
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
        playerSound = GetComponentInChildren<PlayerSound>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Path != null)
        {
            if (grinding)
            {
                playerSound.PlaySoundEffect(PlayerSound.voiceSounds.NONE, PlayerSound.sounds.GRIND);
                charStats.Air += airGain;

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
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    if (!jumpPressed)
                    {
                        Vector3.ClampMagnitude(velocity, charStats.GetCurrentBoost());
                        rb.velocity = velocity;
                    }
                    else
                    {
                        rb.velocity = transform.GetChild(0).forward * jumpSpeed;
                    }
                    
                    movement.CantMove = false;

                    playerSound.StopPlayingGrind();

                    if (jumpPressed)
                    {
                        playerJump.GrindJumpHeight = jumpHeightOfRail;
                        playerJump.JumpRelease = true;
                    }                    
                }

                return;
            }

            if (!movement.Grounded && JumpPressed && !grinding)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
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
