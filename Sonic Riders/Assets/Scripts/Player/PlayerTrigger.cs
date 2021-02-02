using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerCheckpoints playerCheckpoints;
    private Transform playerTransform;
    private PlayerFlight playerFlight;
    private PlayerAnimationHandler playerAnimation;
    public PlayerAnimationHandler PlayerAnimation { get { return playerAnimation; } }
    private PlayerBoost playerBoost;
    private PlayerPunchObstacle playerPunch;
    private AudioManagerHolder audioHolder;
    private Rigidbody rb;
    private PlayerBounce playerBounce;
    private CharacterStats charStats;
    private SurvivalManager survivalManager;

    [SerializeField] private float speed = 2;
    private float closestDistance;
    [SerializeField] private float time = 0.5f;
    private Vector3 bounceDir;

    private PathCreator path;
    private float autoSpeed = 0;
    private float launchSpeed = 0;
    private Transform launchForward;

    private bool touchedStartAlready = false;
    private bool alreadyAttacked = false;
    public bool AlreadyAttacked { set { alreadyAttacked = value; } }
    private float otherPlayerForce = 0;
    private float attackKnockbackMultiplier = 1f;
    [SerializeField] private float attackKnockbackHeight = 10;

    [SerializeField] private List<Collider> attackColls = new List<Collider>();
    [SerializeField] private Transform emeraldHolder;
    public Transform EmeraldHolder { get { return emeraldHolder; } }

    private bool losingEmerald = false;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerTransform = playerMovement.transform;
        playerFlight = playerMovement.GetComponent<PlayerFlight>();
        playerAnimation = playerMovement.GetComponent<PlayerAnimationHandler>();
        playerBoost = playerMovement.GetComponent<PlayerBoost>();
        playerCheckpoints = playerMovement.GetComponent<PlayerCheckpoints>();
        playerPunch = playerMovement.GetComponent<PlayerPunchObstacle>();
        charStats = playerMovement.GetComponent<CharacterStats>();
        audioHolder = playerMovement.GetComponent<AudioManagerHolder>();
        rb = GetComponentInParent<Rigidbody>();
        playerBounce = playerMovement.GetComponentInChildren<PlayerBounce>();

        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            survivalManager = FindObjectOfType<SurvivalManager>();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.gameObject);

        bool tagFound = false;

        switch (collision.gameObject.tag)
        {
            case Constants.Tags.obstacle:
                if ((!playerPunch.CantPunch && charStats.Air > 0) || charStats.Invincible)
                {
                    tagFound = true;
                    playerPunch.Punch(collision.attachedRigidbody, 0);
                }
                break;
            case Constants.Tags.eggPawn:
                tagFound = true;

                if ((!playerPunch.CantPunch && charStats.Air > 0) || playerBoost.Attacking || charStats.Invincible)
                {
                    playerPunch.Punch(collision.attachedRigidbody, 5);
                }
                else
                {
                    playerBounce.Attacked(collision.transform.position, rb.velocity.magnitude);
                }
                break;
            case Constants.Tags.enemy:
                tagFound = true;
                playerBounce.Attacked(collision.transform.position, rb.velocity.magnitude);
                break;
        }

        if (tagFound)
        {
            return;
        }

        switch (collision.gameObject.layer)
        {
            case 4:
                if (playerMovement.Grounded && !playerMovement.OnWater && !playerMovement.UnderWater)
                {
                    playerMovement.GoUnderSea(true);
                }

                break;
            case 8:
                if (collision.isTrigger && !attackColls.Contains(collision) && !alreadyAttacked)
                {
                    AttackedByPlayer(collision);
                }
                break;
            case 9:
                switch (collision.gameObject.tag)
                {
                    case Constants.Tags.startLine:
                        if (!touchedStartAlready)
                        {
                            touchedStartAlready = true;
                            CheckCountdown(collision.gameObject.GetComponent<StartingLevel>());
                        }
                        break;
                    case Constants.Tags.checkpoint:
                        playerCheckpoints.CheckCheckpoint(collision.transform);
                        break;
                    case Constants.Tags.scoreRing:
                        survivalManager.CheckValidScoreRing(collision.transform.parent.gameObject, playerMovement.gameObject);
                        break;
                    case Constants.Tags.flyPortal:
                        if (playerFlight.enabled)
                        {
                            playerFlight.IncreaseFlightSpeed(collision.transform.parent);
                        }
                        break;
                    case Constants.Tags.auto:
                        if (charStats.Air == 0 && path == null)
                        {
                            Transform spring = collision.transform.parent;
                            path = spring.GetComponentInChildren<PathCreator>();

                            SpringStats springStats = spring.GetComponent<SpringStats>();

                            autoSpeed = springStats.Speed;
                            launchForward = springStats.Forward;
                            launchSpeed = springStats.LaunchSpeed;

                            StartCoroutine("FollowPath");
                        }
                        break;
                }

                break;
            case 10:
                GotChaosEmerald(collision.transform.parent);
                break;
        }       
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == 4 && playerMovement.Sea != null && transform.position.y > playerMovement.Sea.position.y)
        {
            playerMovement.AboveSea(true);
        }
    }

    private void GotChaosEmerald(Transform chaosEmerald)
    {
        if (!chaosEmerald.GetComponent<ChaosEmerald>().CanCatch)
        {
            return;
        }

        survivalManager.MakePlayerLeader(playerMovement.gameObject);

        chaosEmerald.SetParent(emeraldHolder, false);
        chaosEmerald.localPosition = Vector3.zero;
        chaosEmerald.forward = transform.forward;
        chaosEmerald.GetChild(0).localScale = new Vector3(0.5f, 0.5f, 0.5f);
        playerBoost.Attacking = false;
        playerAnimation.Anim.SetBool("Boosting", false);
    }

    public void AttackedByPlayer(Collider collision)
    {
        if (charStats.Invincible || (charStats.SuperForm && charStats.Air > 0 && GameManager.instance.GameMode != GameManager.gamemode.SURVIVAL))
        {
            return;
        }

        PlayerBoost attackerBoost = collision.attachedRigidbody.GetComponent<PlayerBoost>();
        CharacterStats attackerStats = attackerBoost.GetComponent<CharacterStats>();

        if (!attackerBoost.AttackCol.activeInHierarchy && !attackerBoost.Attacking && !attackerStats.Invincible)
        {
            return;
        }

        //Debug.Log("Me: " + rb.gameObject.name + " attacker: " + attackerBoost.gameObject.name + " col: " + collision + " true?: " + (!attackerBoost.AttackCol.activeInHierarchy));

        alreadyAttacked = true;
        
        PlayerTrigger attackerTrigger = attackerBoost.GetComponentInChildren<PlayerTrigger>();

        if (emeraldHolder.childCount > 0)
        {
            charStats.Air = charStats.MaxAir;

            survivalManager.MakePlayerLeader(attackerBoost.gameObject);

            Transform emerald = emeraldHolder.GetChild(0);

            emerald.SetParent(attackerTrigger.EmeraldHolder, false);
            emerald.localPosition = Vector3.zero;
            emerald.forward = transform.forward;
            emerald.GetChild(0).localScale = new Vector3(0.5f, 0.5f, 0.5f);
            attackerBoost.Attacking = false;
            attackerTrigger.PlayerAnimation.Anim.SetBool("Boosting", false);
        }

        playerBounce.Attacked(attackColls[0].transform.position, attackerBoost.GetComponent<PlayerMovement>().Speed);
    }

    public void Electrocute(float timer, bool countDown)
    {
        audioHolder.SfxManager.Play(Constants.SoundEffects.electrocuted);
        losingEmerald = !countDown;
        playerMovement.CantMove = true;
        playerAnimation.Anim.SetBool("Electrocuted", true);
        charStats.DisableAllFeatures = true;
        playerMovement.Speed = 0;
        rb.velocity = Vector3.zero;
        float waitTime = 2;        

        if (countDown)
        {
            if (timer > 2)
            {
                waitTime = timer;
            }
        }

        Invoke("CanRun", waitTime);
    }

    private void CheckCountdown(StartingLevel startingLevel)
    {
        if (charStats.BoardStats.RingsAsAir)
        {
            if (GameManager.instance.GameMode != GameManager.gamemode.SURVIVAL)
            {
                charStats.Rings = charStats.BoardStats.StartingAir;
            }
            else
            {
                charStats.Rings = 100;
            }
        }
        else
        {
            charStats.Air = charStats.BoardStats.StartingAir;
        }

        if (startingLevel.Timer > 0)
        {
            Electrocute(startingLevel.Timer, true);
        }
        else
        {
            float timeAfterCrossing = startingLevel.Timer;

            if (timeAfterCrossing <= -1)
            {
                return;
            }

            double timeRounded = System.Math.Round(timeAfterCrossing, 1);

            float timePercent = (float)timeRounded;

            timePercent = -timePercent;

            float percent = 1 - timePercent;

            float extraSpeed = 33 * percent;

            Debug.Log("Extra speed: " + extraSpeed + " time:" + timeRounded);

            playerMovement.Speed += extraSpeed;           
        }
    }

    private void CanRun()
    {
        audioHolder.SfxManager.StopPlaying(Constants.SoundEffects.electrocuted);
        playerAnimation.Anim.SetBool("Electrocuted", false);
        playerMovement.CantMove = false;
        charStats.DisableAllFeatures = false;

        if (losingEmerald)
        {
            charStats.Air = charStats.MaxAir;
        }
    }

    private IEnumerator FollowPath()
    {
        if (path != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            playerMovement.CantMove = true;
            speed = playerMovement.Speed;
            closestDistance = path.path.GetClosestDistanceAlongPath(playerTransform.position);
            Quaternion rot = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);
            rot.x = 0;
            rot.z = 0;
            playerTransform.GetChild(0).localRotation = rot;
            playerTransform.position = path.path.GetClosestPointOnPath(playerTransform.position);

            while (path.path.GetClosestTimeOnPath(playerTransform.position) < 0.99f)
            {
                closestDistance += autoSpeed * Time.deltaTime;
                Vector3 desiredPos = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);
                //desiredPos += transform.GetChild(0).TransformDirection(0, extraCharHeight, 0);
                playerTransform.position = desiredPos;

                Quaternion localRot = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);
                localRot.x = 0;
                localRot.z = 0;
                playerTransform.GetChild(0).localRotation = localRot;
                yield return null;
            }

            path = null;

            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            playerTransform.GetChild(0).forward = launchForward.forward;
            playerTransform.GetChild(0).localRotation = new Quaternion(0, playerTransform.GetChild(0).localRotation.y, 0, playerTransform.GetChild(0).localRotation.w);

            rb.velocity = launchForward.forward * launchSpeed;

            playerMovement.CantMove = false;
        }        
    }

    /*public void BounceCol(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (!alreadyAttacked)
            {
                otherPlayerForce = collision.GetComponentInParent<PlayerMovement>().Speed * attackKnockbackMultiplier;

                if (survivalManager == null)
                {
                    charStats.Rings -= 20;
                }

                alreadyAttacked = true;
            }
            else
            {
                return;
            }
        }        

        playerMovement.Bouncing = true;

        Vector3 closestPoint = Vector3.zero;

        if (collision.GetComponent<MeshCollider>() != null)
        {
            closestPoint = NearestVertexTo(transform.position, collision.GetComponent<MeshFilter>().mesh);
        }
        else
        {
            closestPoint = collision.ClosestPoint(transform.position);
        }

        bounceDir = (transform.position - closestPoint).normalized;
        StartCoroutine("Bounce");
    }

    public Vector3 NearestVertexTo(Vector3 point, Mesh mesh)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return transform.TransformPoint(nearestVertex);
    }*/
}
