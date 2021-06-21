using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerAnimationHandler playerAnimation;
    private PlayerJump playerJump;
    private PlayerPunchObstacle playerPunch;
    private PlayerBoost playerBoost;
    private PlayerTrigger playerTrigger;
    private CharacterStats charStats;
    private TurbulenceRider turbulenceRider;

    private AudioManagerHolder audioHolder;

    [SerializeField] private Vector3 bounceDir;

    private float speed = 0;
    [SerializeField] private float time = 0.25f;
    private bool attacked = false;
    private bool obstacle = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        rb = playerMovement.GetComponent<Rigidbody>();
        playerAnimation = rb.GetComponent<PlayerAnimationHandler>();
        playerJump = rb.GetComponent<PlayerJump>();
        playerBoost = rb.GetComponent<PlayerBoost>();
        playerTrigger = rb.GetComponentInChildren<PlayerTrigger>();
        playerPunch = rb.GetComponent<PlayerPunchObstacle>();
        charStats = rb.GetComponent<CharacterStats>();
        audioHolder = rb.GetComponent<AudioManagerHolder>();
        turbulenceRider = rb.GetComponent<TurbulenceRider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerMovement.Bouncing || charStats.DontAlign || collision.gameObject.layer == 14 || collision.gameObject.CompareTag(Constants.Tags.eggPawn) || collision.gameObject.CompareTag(Constants.Tags.debris) || collision.gameObject.CompareTag(Constants.Tags.loopWall))
        {
            return;
        }

        obstacle = false;

        if (collision.gameObject.CompareTag(Constants.Tags.obstacle))
        {

            if ((!playerPunch.CantPunch && charStats.Air > 0) || charStats.Invincible)
            {
                //Debug.Log("NO BOUNCE OFF OF" + collision.gameObject);
                return;
            }
            else
            {
                //Debug.Log("BOUNCE OFF OF" + collision.gameObject);
                obstacle = true;
            }
        }

        //Debug.Log(collision.gameObject);

        speed = rb.velocity.magnitude;
        Vector3 contactNormal = collision.contacts[0].normal;
        
        bounceDir = Vector3.Reflect(rb.velocity.normalized, contactNormal).normalized;

        attacked = false;
        StartCoroutine("Bounce");

        playerMovement.CantMove = true;       
    }
    

    public void Attacked()
    {
        if (charStats.Invincible || attacked || playerMovement.Bouncing)
        {
            return;
        }

        if (charStats.Rings > 0)
        {
            if (!charStats.SuperForm || charStats.Air == 0)
            {
                charStats.Rings -= 20;
                audioHolder.SfxManager.Play(Constants.SoundEffects.ringLoss);
            }
        }

        audioHolder.VoiceManager.Play(Constants.VoiceSounds.hit);

        if (rb.isKinematic)
        {
            playerMovement.Speed = 0;
            return;
        }

        attacked = true;

        speed = 20; //attackedSpeed;

        bounceDir = -transform.forward; //(transform.position - attackerPos).normalized;

        StartCoroutine("Bounce");
    }

    private IEnumerator Bounce()
    {
        if (turbulenceRider.InTurbulence)
        {
            turbulenceRider.OutTurbulence();
        }

        if (!attacked)
        {
            playerMovement.CantMove = true;
        }
        else
        {
            charStats.DisableAllFeatures = true;
        }

        playerMovement.Bouncing = true;

        //Debug.Log(transform.InverseTransformDirection(bounceDir).z);

        bool hitDirectly = false;

        float diffAngle = Vector3.SignedAngle(transform.forward, bounceDir, transform.up);

        Debug.Log("Forward: " + transform.forward + " Bounce dir: " + bounceDir + " diff: " + diffAngle);

        if (diffAngle > 70)
        {
            playerMovement.Speed = 0;

            hitDirectly = true;
        }

        float knockback = speed;

        if (!hitDirectly && playerMovement.Grounded && !attacked && !obstacle)
        {
            if (playerMovement.Speed > 30)
            {
                rb.transform.GetChild(0).forward = bounceDir;

                Quaternion newLocalRot = rb.transform.GetChild(0).localRotation;
                newLocalRot.x = 0;
                newLocalRot.z = 0;

                rb.transform.GetChild(0).localRotation = newLocalRot;

                playerMovement.Speed *= 0.75f;
            }
            else
            {
                playerMovement.Speed = 0;
            }            
        }       

        rb.velocity = bounceDir * Mathf.Max(speed, 0);        

        if (attacked)
        {
            playerAnimation.Anim.SetBool("GotHit", true);
            playerMovement.RaycastLength = 0.1f;
            playerJump.StartCanDragDown();
            playerMovement.Attacked = true;
            playerMovement.Speed = 0;
            rb.AddForce(transform.up * 5, ForceMode.Impulse);
        }
        else
        {
            audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);
        }

        yield return new WaitForSeconds(time / 2);
        playerMovement.Bouncing = false;        
        yield return new WaitForSeconds(time / 2);

        if (hitDirectly && playerMovement.Grounded)
        {
            playerMovement.Speed = 0;
            rb.velocity = Vector3.zero;
        }

        if (!attacked)
        {            
            playerMovement.CantMove = false;
        }

        playerTrigger.AlreadyAttacked = false;
        attacked = false;
    }
}
