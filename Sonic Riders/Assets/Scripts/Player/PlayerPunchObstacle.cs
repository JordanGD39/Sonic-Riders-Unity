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
    private bool rightPunch = true;

    private Rigidbody lastPunched;

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

    public void Punch(Rigidbody obstacleRb, float upperPower)
    {
        EggPawnAI ai = obstacleRb.GetComponent<EggPawnAI>();

        if (ai != null)
        {
            ai.Die();
        }

        float powerCalc = charStats.ExtraPower * extraPowerMultiplier;

        if (powerCalc < 0)
        {
            powerCalc = 0;
        }

        //Removing speed loss in formula so that slower characters punch less hard but adding power so that Powerful characters punch harder
        float maxSpeed = charStats.GetCurrentLimit() + powerCalc - charStats.ExtraSpeed;

        float speedPowerCalc = playerMovement.Speed / maxSpeed;

        if ((!CantPunch && charStats.Air > 0) || charStats.Invincible)
        {
            if (playerMovement.Grounded && charStats.Air > 0)
            {
                if (rightPunch)
                {
                    playerAnimation.Anim.SetTrigger("Punch");
                }
                else
                {
                    playerAnimation.Anim.SetTrigger("LeftPunch");
                }

                playerAnimation.Anim.SetBool("Punching", true);
            }

            Transform punch = rightPunchAngle;

            if (!rightPunch)
            {
                punch = leftPunchAngle;
            }

            float power = punchPower * speedPowerCalc;

            obstacleRb.AddForce(punch.forward * power, ForceMode.VelocityChange);

            if (upperPower > 0)
            {
                Debug.Log(upperPower);
                obstacleRb.AddForce(obstacleRb.transform.up * upperPower, ForceMode.Impulse);
            }
            
            audioHolder.SfxManager.Play(Constants.SoundEffects.punch);

            rightPunch = !rightPunch;

            if (!charStats.BoardStats.RingsAsAir && obstacleRb != lastPunched)
            {
                charStats.Air += 20;
            }

            lastPunched = obstacleRb;
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
