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

    private AudioManagerHolder audioHolder;

    [SerializeField] private Vector3 bounceDir;

    private float speed = 0;
    [SerializeField] private float time = 0.25f;
    private bool attacked = false;
    private bool obstacle = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponentInParent<Rigidbody>();
        playerMovement = rb.GetComponent<PlayerMovement>();
        playerAnimation = rb.GetComponent<PlayerAnimationHandler>();
        playerJump = rb.GetComponent<PlayerJump>();
        playerBoost = rb.GetComponent<PlayerBoost>();
        playerTrigger = rb.GetComponentInChildren<PlayerTrigger>();
        playerPunch = rb.GetComponent<PlayerPunchObstacle>();
        charStats = rb.GetComponent<CharacterStats>();
        audioHolder = rb.GetComponent<AudioManagerHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attacked(playerMovement.transform.position + new Vector3(0,0,1), playerMovement.Speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        obstacle = false;

        if (collision.gameObject.layer == 11)
        {
            if (!playerPunch.CantPunch && charStats.Air > 0)
            {
                return;
            }
            else
            {
                obstacle = true;
            }
        }

        Debug.Log(collision.gameObject);

        speed = rb.velocity.magnitude;
        bounceDir = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        attacked = false;
        StartCoroutine("Bounce");

        playerMovement.CantMove = true;       
    }

    public void Attacked(Vector3 attackerPos, float attackedSpeed)
    {
        attacked = true;

        speed = attackedSpeed;

        bounceDir = (transform.position - attackerPos).normalized;

        StartCoroutine("Bounce");
    }

    private IEnumerator Bounce()
    {
        playerMovement.CantMove = true;
        playerMovement.Bouncing = true;

        //Debug.Log(transform.InverseTransformDirection(bounceDir).z);

        bool hitDirectly = false;

        Vector3 localBounce = transform.InverseTransformDirection(bounceDir);

        if (localBounce.z < localBounce.x)
        {
            playerMovement.Speed = speed;
            hitDirectly = true;
        }

        float knockback = speed;

        localBounce.y = 0;

        bounceDir = transform.TransformDirection(localBounce);

        if (!hitDirectly && playerMovement.Grounded && !attacked && !obstacle)
        {
            playerMovement.transform.GetChild(0).forward = bounceDir;

            Quaternion newLocalRot = playerMovement.transform.GetChild(0).localRotation;
            newLocalRot.x = 0;
            newLocalRot.z = 0;

            playerMovement.transform.GetChild(0).localRotation = newLocalRot;
        }       

        rb.velocity = bounceDir * Mathf.Max(speed, 0);

        if (attacked)
        {
            if (charStats.Rings > 0)
            {
                audioHolder.SfxManager.Play(Constants.SoundEffects.ringLoss);
            }

            audioHolder.VoiceManager.Play(Constants.VoiceSounds.hit);

            playerAnimation.Anim.SetBool("GotHit", true);
            playerMovement.RaycastLength = 0.1f;
            playerJump.StartCanDragDown();
            playerMovement.Attacked = true;
            playerMovement.Speed = 0;
            rb.AddForce(transform.up * 5, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(time / 2);
        playerMovement.Bouncing = false;
        yield return new WaitForSeconds(time / 2);

        if (!attacked)
        {
            audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);
            playerMovement.CantMove = false;
        }
        playerTrigger.AlreadyAttacked = false;

        if (hitDirectly && playerMovement.Grounded)
        {
            playerMovement.Speed = 0;
            rb.velocity = Vector3.zero;
        }
    }
}
