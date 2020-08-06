using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerGrind playerGrind;
    private PlayerJump playerJump;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerJump = GetComponent<PlayerJump>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = playerMovement.Speed * 0.03f;

        if (speed > 3)
        {
            speed = 3;
        }
        else if (speed < 0)
        {
            speed = 0;
        }

        anim.SetFloat("Speed", speed);
        anim.SetFloat("Direction", playerMovement.TurnAmount);
        anim.SetBool("Grinding", playerGrind.Grinding);
        anim.SetBool("Grounded", playerMovement.Grounded);
        anim.SetBool("ChargingJump", playerJump.JumpHold);
    }

    public void StartBoostAnimation()
    {
        anim.SetTrigger("Boost");
    }
}
