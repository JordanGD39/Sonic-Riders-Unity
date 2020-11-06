using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private bool notReady = false;
    public Animator Anim { get { return anim; } }
    private PlayerMovement playerMovement;
    private PlayerBoost playerBoost;
    private PlayerGrind playerGrind;
    private PlayerJump playerJump;
    private PlayerTricks playerTricks;
    private PlayerFlight playerFlight;

    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private bool diffFlyAnim = false;
    [SerializeField] private bool moveTails = false;
    [SerializeField] private bool changeBoostSpeed = false;

    public bool AlreadySettingAttack { get; set; } = false; 

    private bool boosting = true;

    private bool updateBoost = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerBoost = GetComponent<PlayerBoost>();
        playerGrind = GetComponent<PlayerGrind>();
        playerJump = GetComponent<PlayerJump>();
        playerTricks = GetComponent<PlayerTricks>();
        playerFlight = GetComponent<PlayerFlight>();

        if (playerBoost.AttackCol != null)
        {
            playerBoost.AttackCol.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (notReady)
        {
            return;
        }
        
        float speed = playerMovement.Speed * 0.03f;
        float runSpeed = speed * runSpeedMultiplier;
        float boostSpeed = playerMovement.Speed / 66.67f;

        speed = Mathf.Clamp(speed, 0, 3);
        runSpeed = Mathf.Clamp(runSpeed, -4, 4);

        if (anim.layerCount > 1 || diffFlyAnim)
        {
            anim.SetFloat("Speed", speed * speedMultiplier);
        }

        anim.SetFloat("RunSpeed", runSpeed);

        if (changeBoostSpeed)
        {
            anim.SetFloat("BoostSpeed", boostSpeed);
        }
        anim.SetFloat("Direction", playerMovement.TurnAmount);
        anim.SetBool("Grinding", playerGrind.Grinding);
        anim.SetBool("Grounded", playerMovement.Grounded);
        anim.SetBool("ChargingJump", playerJump.JumpHold);        

        if (!AlreadySettingAttack && !playerBoost.AttackAnim)
        {
            playerBoost.AttackCol.SetActive(anim.GetCurrentAnimatorStateInfo(0).IsName("BoostAttack"));
        }

        if (playerTricks.CanDoTricks)
        {
            anim.SetFloat("TrickVerticalDir", playerTricks.TrickDirection.y);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                anim.SetBool("DoingTricks", playerTricks.CanDoTricks);
            }
        }        
        else
        {
            anim.SetFloat("TrickVerticalDir", 0);
        }

        if (diffFlyAnim)
        {
            anim.SetBool("Flying", playerFlight.Flying);
        }
    }

    public void RunningState(bool state)
    {
        anim.SetBool("OutOfAir", state);
    }

    public void StartBoostAnimation(bool canAttack)
    {
        anim.SetTrigger("Boost");

        if (canAttack)
        {
            anim.SetBool("Boosting", true);
        }

        if (moveTails)
        {
            anim.SetTrigger("TailsStop");
        }
    }
}
