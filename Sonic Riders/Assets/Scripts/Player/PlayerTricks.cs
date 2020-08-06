using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTricks : MonoBehaviour
{
    public bool CanDoTricks { get; set; } = false;
    public Vector2 TrickDirection { get; set; }

    private PlayerMovement playerMovement;
    private PlayerSound playerSound;
    private PlayerAnimationHandler playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerSound = GetComponentInChildren<PlayerSound>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
    }

    public void ChangeTrickSpeed(float rampPower, float maxRampPower, float worstPower, float jumpHeight, float startingJumpHeight, float maxJumpHeight)
    {
        CanDoTricks = true;

        float rampDiff = maxRampPower - worstPower;
        float jumpDiff = maxJumpHeight - startingJumpHeight;

        float rampTiming = (rampPower - worstPower) / rampDiff;
        float jumpCharge = (jumpHeight - startingJumpHeight)/ jumpDiff;
        rampTiming *= 0.7f;
        jumpCharge *= 0.3f;

        float speed = rampTiming + jumpCharge;

        if (speed < 0.25f)
        {
            speed = 0.25f;
        }

        playerAnimation.Anim.SetFloat("TrickSpeed", speed);
        Debug.Log("Trick speed: " + speed);
        transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);
    }

    // Update is called once per frame
    public void Landed()
    {
        if (!playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
        {
            playerMovement.Speed *= 0.1f;
            Debug.Log("Trick speed loss!!!");           
        }
        transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);
        playerAnimation.Anim.SetBool("DoingTricks", false);

        CanDoTricks = false;
    }
}
