using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOnAnimation : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerBoost playerBoost;
    private PlayerMovement playerMovement;
    private PlayerTricks playerTricks;
    private PlayerAnimationHandler playerAnimation;
    private PlayerPunchObstacle playerPunchObstacle;
    private CharacterStats characterStats;
    private Rigidbody rb;

    private bool gainAir = false;
    private float airGainSpeed = 32;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerBoost = playerMovement.GetComponent<PlayerBoost>();
        playerTricks = playerMovement.GetComponent<PlayerTricks>();
        playerAnimation = playerMovement.GetComponent<PlayerAnimationHandler>();
        playerPunchObstacle = playerMovement.GetComponent<PlayerPunchObstacle>();
        characterStats = playerMovement.GetComponent<CharacterStats>();
        rb = playerMovement.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (gainAir)
        {
            if (!playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("Swimming"))
            {
                gainAir = false;
            }

            characterStats.Air += airGainSpeed * Time.deltaTime;
        }
    }

    public void Swimming()
    {        
        gainAir = true;
    }

    public void StopSwimming()
    {
        characterStats.DisableAllFeatures = false;
        playerMovement.Speed = 30;
        rb.velocity = playerMovement.transform.GetChild(0).forward * 30;
        playerMovement.JustDied = false;
        gainAir = false;
    }

    public void BoostNow()
    {
        if (!playerMovement.Grounded)
        {
            playerBoost.Boosting = false;
            return;
        }

        playerBoost.Boost(); 
    }

    public void TriggerRightPunch()
    {
        playerPunchObstacle.RightPunch = true;
    }

    public void TriggerLeftPunch()
    {
        playerPunchObstacle.RightPunch = false;
    }

    public void PunchDone()
    {
        playerAnimation.Anim.SetBool("Punching", false);
    }

    public void CountTrick()
    {
        //Debug.Log("YESSS");
        playerTricks.TrickCountUp();
        playerTricks.CanLand = true;
    }

    public void CantLand()
    {
        playerTricks.CanLand = false;
    }

    public void GotHitFalse()
    {
        playerAnimation.Anim.ResetTrigger("Moved");
        playerAnimation.Anim.SetBool("GotHit", false);
    }

    public void HitDone()
    {        
        playerMovement.JustDied = false;
        playerMovement.CantMove = false;
        characterStats.DisableAllFeatures = false;
    }
}
