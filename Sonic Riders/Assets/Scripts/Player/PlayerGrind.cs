using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerGrind : MonoBehaviour
{
    private AudioManagerHolder audioHolder;
    private PlayerMovement movement;
    private PlayerTricks playerTricks;
    private PlayerJump playerJump;
    private Rigidbody rb;
    private BoardStats stats;
    private CharacterStats charStats;

    //public bool JumpPressed { get; set; } = false;

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
    private float distanceMultiplier = 5;

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
        playerTricks = GetComponent<PlayerTricks>();
        playerJump = GetComponent<PlayerJump>();
        rb = GetComponent<Rigidbody>();
        stats = charStats.BoardStats;
        //hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<HUD>();
        audioHolder = GetComponent<AudioManagerHolder>();
    }

    public void GiveCanvasHud()
    {
        hud = charStats.Canvas.GetComponent<HUD>();
    }

    // Update is called once per frame
    void Update()
    {
        if (path != null)
        {
            if (grinding)
            {
                audioHolder.SfxManager.Play(Constants.SoundEffects.grind);
                charStats.Air += airGain * Time.deltaTime;

                Vector3 distance = transform.position - previousPos;

                velocity = distance / Time.deltaTime;
                previousPos = transform.position;

                //distance.y *= distanceMultiplier;

                //Debug.Log(distance.y);

                //Debug.Log(transform.GetChild(0).localRotation.x);

                movement.Speed += 20 * transform.GetChild(0).localRotation.x * Time.deltaTime;                    

                if (movement.Speed <= 4 && movement.Speed >= 0)
                {
                    movement.Speed = -4;
                }

                speed = movement.Speed;

                hud.UpdateSpeedText(speed);

                closestDistance += speed * Time.deltaTime;
                Vector3 desiredPos = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
                desiredPos += transform.GetChild(0).TransformDirection(0, extraCharHeight, 0);
                transform.position = desiredPos;
                transform.GetChild(0).localRotation = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);

                if (path.path.GetClosestTimeOnPath(transform.position) == 0 && speed < 0)
                {
                    movement.Speed = 20;
                }

                if (path.path.GetClosestTimeOnPath(transform.position) > 0.99f)
                {
                    OffRail(false);                    
                }
            }                        
        }        
    }

    public void CheckGrind()
    {
        //Jumping on Rail
        if (!movement.Grounded && !grinding && path != null && path.path.GetClosestTimeOnPath(transform.position) < 0.99f)
        {
            if (playerTricks.CanDoTricks)
            {
                playerTricks.Landed(false);
            }
            transform.GetChild(0).GetChild(0).localRotation = new Quaternion(0, 0, 0, 0);
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            movement.CantMove = true;
            speed = movement.Speed;
            closestDistance = path.path.GetClosestDistanceAlongPath(transform.position);
            transform.GetChild(0).localRotation = Quaternion.LookRotation(path.path.GetDirectionAtDistance(closestDistance, EndOfPathInstruction.Stop));
            transform.position = path.path.GetClosestPointOnPath(transform.position);
            grinding = true;
        }
        else if (grinding)
        {
            OffRail(true);
        }
    }

    private void OffRail(bool jumpPressed)
    {
        Debug.Log("OffRail " + jumpPressed);
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
            int dir = 1;

            if (speed < 0)
            {
                dir = -1;
            }

            rb.velocity = transform.GetChild(0).forward * (jumpSpeed * dir);
        }

        movement.CantMove = false;

        audioHolder.SfxManager.StopPlaying(Constants.SoundEffects.grind);

        if (jumpPressed)
        {
            playerJump.GrindJumpHeight = jumpHeightOfRail;
            playerJump.RampPower = 0;
            playerJump.JumpRelease = true;
        }
    }
}
