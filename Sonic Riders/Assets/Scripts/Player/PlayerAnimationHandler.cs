using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public Animator Anim { get { return anim; } }
    private PlayerMovement playerMovement;
    private PlayerGrind playerGrind;
    private PlayerJump playerJump;
    private PlayerTricks playerTricks;

    [SerializeField] private float runSpeedMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerJump = GetComponent<PlayerJump>();
        playerTricks = GetComponent<PlayerTricks>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim == null)
        {
            return;
        }

        float speed = playerMovement.Speed * 0.03f;
        float runSpeed = speed * runSpeedMultiplier;

        speed = Mathf.Clamp(speed, 0, 3);
        runSpeed = Mathf.Clamp(runSpeed, -4, 4);

        anim.SetFloat("Speed", speed);
        anim.SetFloat("RunSpeed", runSpeed);
        anim.SetFloat("Direction", playerMovement.TurnAmount);
        anim.SetBool("Grinding", playerGrind.Grinding);
        anim.SetBool("Grounded", playerMovement.Grounded);
        anim.SetBool("ChargingJump", playerJump.JumpHold);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") && playerTricks.CanDoTricks)
        {
            anim.SetBool("DoingTricks", playerTricks.CanDoTricks);
        }

        if (playerTricks.CanDoTricks)
        {
            anim.SetFloat("TrickVerticalDir", playerTricks.TrickDirection.y);
        }        
        else
        {
            anim.SetFloat("TrickVerticalDir", 0);
        }
    }

    public void RunningState(bool state)
    {
        anim.SetBool("OutOfAir", state);
    }

    public void StartBoostAnimation()
    {
        anim.SetTrigger("Boost");
    }
}
