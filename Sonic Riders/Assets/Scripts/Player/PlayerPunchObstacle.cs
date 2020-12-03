using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunchObstacle : MonoBehaviour
{
    private CharacterStats charStats;
    private AudioManagerHolder audioHolder;
    private PlayerAnimationHandler playerAnimation;
    private PlayerMovement playerMovement;
    [SerializeField] private Transform rightPunchAngle;
    [SerializeField] private Transform leftPunchAngle;
    [SerializeField] private float punchPower = 20;
    [SerializeField] private float cantPunchMultiplier = 0.5f;
    [SerializeField] private float extraPowerMultiplier = 50;

    public bool CantPunch { get; set; } = true;
    public bool RightPunch { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();
        audioHolder = GetComponent<AudioManagerHolder>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        playerMovement = GetComponent<PlayerMovement>();

        if (charStats.TypeCheck(type.POWER))
        {
            CantPunch = false;
        }
    }

    public void Punch(Rigidbody obstacleRb)
    {
        float powerCalc = charStats.ExtraPower * extraPowerMultiplier;

        if (powerCalc < 0)
        {
            powerCalc = 0;
        }

        //Removing speed loss in formula so that slower characters punch less hard but adding power so that Powerful characters punch harder
        float maxSpeed = charStats.GetCurrentLimit() + powerCalc + charStats.SpeedLoss;

        float speedPowerCalc = playerMovement.Speed / maxSpeed;

        if ((!CantPunch && charStats.Air > 0) || charStats.Invincible)
        {
            if (playerMovement.Grounded && charStats.Air > 0)
            {
                playerAnimation.Anim.SetTrigger("Punch");
                playerAnimation.Anim.SetBool("Punching", true);
            }

            Transform punch = rightPunchAngle;

            if (!RightPunch)
            {
                punch = leftPunchAngle;
            }

            float power = punchPower * speedPowerCalc;

            obstacleRb.AddForce(punch.forward * power);
            
            audioHolder.SfxManager.Play(Constants.SoundEffects.punch);

            if (!charStats.BoardStats.RingsAsAir)
                charStats.Air += 20;
        }
        else
        {
            audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);
            obstacleRb.AddForce((obstacleRb.transform.position - transform.position).normalized * (punchPower * cantPunchMultiplier * speedPowerCalc));
        }

        RespawnObstacle respawn = obstacleRb.GetComponent<RespawnObstacle>();

        if (respawn != null)
        {
            respawn.Punched();
        }
    }
}
