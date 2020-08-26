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

    public bool CantPunch { get; set; } = true;

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

        if (CantPunch)
        {
            punchPower *= cantPunchMultiplier;
        }
    }

    public void Punch(Rigidbody obstacleRb)
    {
        obstacleRb.isKinematic = false;
        if (!CantPunch && charStats.Air > 0)
        {
            obstacleRb.AddForce(rightPunchAngle.forward * punchPower);
            audioHolder.SfxManager.Play(Constants.SoundEffects.punch);

            if (playerMovement.Grounded)
            {
                playerAnimation.Anim.SetTrigger("Punch");
            }
        }
        else
        {
            audioHolder.SfxManager.Play(Constants.SoundEffects.bounceWall);
            obstacleRb.AddForce(transform.forward * punchPower);
        }
    }
}
