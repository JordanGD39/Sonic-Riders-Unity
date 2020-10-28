using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerPunchObstacle playerPunch;
    private PlayerBoost playerBoost;
    private PlayerTrigger playerTrigger;
    private CharacterStats charStats;

    private AudioManagerHolder audioHolder;

    [SerializeField] private Vector3 bounceDir;

    private float speed = 0;
    [SerializeField] private float time = 0.25f;
    private bool attacked = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponentInParent<Rigidbody>();
        playerMovement = rb.GetComponent<PlayerMovement>();
        playerBoost = rb.GetComponent<PlayerBoost>();
        playerTrigger = rb.GetComponentInChildren<PlayerTrigger>();
        playerPunch = rb.GetComponent<PlayerPunchObstacle>();
        charStats = rb.GetComponent<CharacterStats>();
        audioHolder = rb.GetComponent<AudioManagerHolder>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11 && !playerPunch.CantPunch && charStats.Air > 0)
        {
            return;
        }       
        
        speed = rb.velocity.magnitude;
        bounceDir = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        attacked = false;
        StartCoroutine("Bounce");

        playerMovement.CantMove = true;       
    }

    public void Attacked(Transform attacker, float attackedSpeed)
    {
        attacked = true;

        speed = attackedSpeed;

        bounceDir = (transform.position - attacker.position).normalized;

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

        if (!hitDirectly && playerMovement.Grounded && !attacked)
        {
            playerMovement.transform.GetChild(0).forward = bounceDir;

            Quaternion newLocalRot = playerMovement.transform.GetChild(0).localRotation;
            newLocalRot.x = 0;
            newLocalRot.z = 0;

            playerMovement.transform.GetChild(0).localRotation = newLocalRot;
        }

        audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);

        rb.velocity = bounceDir * Mathf.Max(speed, 0);

        if (attacked)
        {
            playerMovement.Speed = 0;
            rb.AddForce(transform.up * 15, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(time / 2);
        playerMovement.Bouncing = false;
        yield return new WaitForSeconds(time / 2);
        playerMovement.CantMove = false;
        playerTrigger.AlreadyAttacked = false;

        if (hitDirectly && playerMovement.Grounded)
        {
            playerMovement.Speed = 0;
            rb.velocity = Vector3.zero;
        }
    }
}
