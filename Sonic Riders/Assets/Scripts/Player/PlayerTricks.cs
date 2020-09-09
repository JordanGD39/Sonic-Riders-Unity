using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTricks : MonoBehaviour
{
    public bool CanDoTricks { get; set; } = false;
    public Vector2 TrickDirection { get; set; }

    private PlayerMovement playerMovement;
    private AudioManagerHolder audioHolder;
    private PlayerAnimationHandler playerAnimation;
    private CharacterStats characterStats;

    private float speedReward;
    [SerializeField] private int tricks = 0;
    public bool CanLand { get; set; } = false;
    
    [SerializeField] private float camSpeed = 1;
    private Rigidbody rb;
    [SerializeField] private Vector3 lowerCamPos;
    [SerializeField] private Vector3 higherCamPos;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioHolder = GetComponent<AudioManagerHolder>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        characterStats = GetComponent<CharacterStats>();
        
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (CanDoTricks && characterStats.IsPlayer)
        {
            Vector3 pos = Vector3.zero;

            if (rb.velocity.y > 0)
            {
                pos = lowerCamPos;
            }
            else
            {
                pos = higherCamPos;
            }

            float step = camSpeed * Time.deltaTime;
            characterStats.Cam.localPosition = Vector3.MoveTowards(characterStats.Cam.localPosition, pos, step);
            characterStats.Cam.LookAt(transform.position);
        }        
    }

    public void TrickCountUp()
    {
        tricks++;
    }

    public void ChangeTrickSpeed(float rampPower, float maxRampPower, float worstPower, float jumpHeight, float startingJumpHeight, float maxJumpHeight)
    {
        CanDoTricks = true;

        float rampDiff = maxRampPower - worstPower;
        float jumpDiff = maxJumpHeight - startingJumpHeight;

        float rampTiming = (rampPower - worstPower) / rampDiff;
        float jumpCharge = (jumpHeight - startingJumpHeight)/ jumpDiff;
        rampTiming *= 0.6f;
        jumpCharge *= 0.4f;

        speedReward = rampTiming + jumpCharge;

        if (speedReward < 0.25f)
        {
            speedReward = 0.25f;
        }

        if (characterStats.IsPlayer)
        {
            playerAnimation.Anim.SetFloat("TrickSpeed", speedReward);
        }
        
        Debug.Log("Trick speed: " + speedReward);
        transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);        
    }

    // Update is called once per frame
    public void Landed(bool landedOnGround)
    {
        if (!characterStats.IsPlayer)
        {
            characterStats.Air += 100;
            CanDoTricks = false;
            transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
            return;
        }

        audioHolder.SfxManager.Play(Constants.SoundEffects.land);

        if (!playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") && !CanLand && landedOnGround)
        {
            audioHolder.VoiceManager.Play(Constants.VoiceSounds.landFail);
            playerMovement.Speed *= 0.1f;
            Debug.Log("Trick speed loss!!!");
            characterStats.Air -= 15;
        }
        else
        {
            characterStats.Air += tricks * 20;

            if (tricks >= 1)
            {
                audioHolder.VoiceManager.Play(Constants.VoiceSounds.landSucces);
                playerMovement.Speed = characterStats.GetCurrentLimit();
            }  
            else
            {
                audioHolder.VoiceManager.Play(Constants.VoiceSounds.landFail);
            }
        }

        tricks = 0;

        transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
        playerAnimation.Anim.SetBool("DoingTricks", false);

        CanDoTricks = false;

        characterStats.Cam.localPosition = characterStats.CamStartPos;
        characterStats.Cam.localRotation = new Quaternion(0, 0, 0, characterStats.Cam.localRotation.w);
    }
}
