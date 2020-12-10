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
    private PlayerFollowPath playerFollowPath;
    private Rigidbody rb;
    private BoardStats stats;
    private CharacterStats charStats;

    //public bool JumpPressed { get; set; } = false;

    [SerializeField] private PathCreator path;
    public VertexPath PathVertex { get; set; }
    public PathCreator Path { get { return path; } set { path = value; } }
    public Quaternion StartingRotation { get; set; }

    [SerializeField] private bool grinding = false;
    public bool Grinding { get { return grinding; } set { grinding = value; } }
    
    [SerializeField] private float jumpSpeed = 20;
    [SerializeField] private float airGain = 0.02f;
    [SerializeField] private float extraCharHeight = 0.2f;
    [SerializeField] private float jumpHeightOfRail = 30;
    private float distanceMultiplier = 5;

    public bool GrindPhysics { get; set; } = false;

    private HUD hud;

    public void GiveCanvasHud()
    {
        charStats = GetComponent<CharacterStats>();

        if (!charStats.TypeCheck(type.SPEED))
        {
            enabled = false;
        }

        movement = GetComponent<PlayerMovement>();
        playerFollowPath = GetComponent<PlayerFollowPath>();
        playerTricks = GetComponent<PlayerTricks>();
        playerJump = GetComponent<PlayerJump>();
        rb = GetComponent<Rigidbody>();
        stats = charStats.BoardStats;
        //hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<HUD>();
        audioHolder = GetComponent<AudioManagerHolder>();
        if (charStats.IsPlayer)
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

                if (!charStats.BoardStats.RingsAsAir)
                {
                    charStats.Air += airGain * Time.deltaTime;
                }

                playerFollowPath.FollowPath(PathVertex, GrindPhysics, extraCharHeight);

                hud.UpdateSpeedText(playerFollowPath.Speed);

                if (PathVertex.GetClosestTimeOnPath(transform.position) == 0 && playerFollowPath.Speed < 0)
                {
                    movement.Speed = 20;
                }

                if (PathVertex.GetClosestTimeOnPath(transform.position) > 0.99f)
                {
                    OffRail(false);                    
                }
            }                        
        }        
    }

    public void CheckGrind(bool grind)
    {
        if (grind && path != null)
        {
            PathVertex = path.path;
            GrindPhysics = true;
        }

        //Jumping on Rail
        if (!movement.Grounded && !grinding && PathVertex != null && PathVertex.GetClosestTimeOnPath(transform.position) < 0.99f)
        {
            if (playerTricks.CanDoTricks)
            {
                playerTricks.Landed(false);
            }
            transform.GetChild(0).GetChild(0).localRotation = new Quaternion(0, 0, 0, 0);
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            movement.CantMove = true;
            playerFollowPath.Speed = movement.Speed;
            playerFollowPath.ClosestDistance = PathVertex.GetClosestDistanceAlongPath(transform.position);
            transform.GetChild(0).localRotation = Quaternion.LookRotation(PathVertex.GetDirectionAtDistance(playerFollowPath.ClosestDistance, EndOfPathInstruction.Stop));
            transform.position = PathVertex.GetClosestPointOnPath(transform.position);
            grinding = true;
        }
        else if (grinding)
        {
            OffRail(true);
        }
    }

    public void OffRail(bool jumpPressed)
    {
        Debug.Log("OffRail " + jumpPressed);
        grinding = false;
        transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (!jumpPressed)
        {
            Vector3.ClampMagnitude(playerFollowPath.Velocity, charStats.GetCurrentBoost());
            rb.velocity = playerFollowPath.Velocity;
        }
        else
        {
            int dir = 1;

            if (playerFollowPath.Speed < 0)
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
