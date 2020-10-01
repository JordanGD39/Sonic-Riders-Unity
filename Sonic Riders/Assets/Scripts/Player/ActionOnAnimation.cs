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
    
    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerBoost = playerMovement.GetComponent<PlayerBoost>();
        playerTricks = playerMovement.GetComponent<PlayerTricks>();
        playerAnimation = playerMovement.GetComponent<PlayerAnimationHandler>();
        playerPunchObstacle = playerMovement.GetComponent<PlayerPunchObstacle>();
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
        Debug.Log("YESSS");
        playerTricks.TrickCountUp();
        playerTricks.CanLand = true;
    }

    public void CantLand()
    {
        playerTricks.CanLand = false;
    }
}
