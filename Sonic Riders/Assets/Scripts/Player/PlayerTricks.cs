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
    private TurbulenceGenerator turbulenceGenerator;
    private CharacterStats characterStats;

    private float speedReward;
    [SerializeField] private int tricks = 0;
    [SerializeField] private float extraSpeed = 0.25f;
    public bool CanLand { get; set; } = false;
    
    [SerializeField] private float camSpeed = 1;
    private Rigidbody rb;
    [SerializeField] private Vector3 lowerCamPos;
    [SerializeField] private Vector3 higherCamPos;
    [SerializeField] private float trickForceUp = 3;
    [SerializeField] private float trickForceForward = 3;
    [SerializeField] private float trickForceSideways = 5;
    private bool rampStatic = false;
    private float turnSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioHolder = GetComponent<AudioManagerHolder>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        turbulenceGenerator = GetComponent<TurbulenceGenerator>();
        characterStats = GetComponent<CharacterStats>();
        
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (CanDoTricks)
        {
            transform.GetChild(0).Rotate(0, TrickDirection.x * turnSpeed * Time.deltaTime, 0);

            if (!characterStats.IsPlayer)
            {
                return;
            }

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
            //Debug.Log(characterStats.Cam.transform.localRotation);
        }
    }

    private void FixedUpdate()
    {
        if (CanDoTricks && !playerMovement.Grounded)
        {
            if (!rampStatic)
            {
                rb.AddForce(transform.up * -TrickDirection.y * trickForceUp);
                rb.AddForce(transform.GetChild(0).forward * TrickDirection.y * trickForceForward);
            }
            
            rb.AddForce(transform.GetChild(0).right * TrickDirection.x * trickForceSideways);
        }
    }

    public void TrickCountUp()
    {
        tricks++;
    }

    public void ChangeTrickSpeed(float jumpHeight, float startingJumpHeight, float maxJumpHeight, bool isRampStatic)
    {
        rampStatic = isRampStatic;
        CanDoTricks = true;

        turbulenceGenerator.PauseGeneration();
        
        float jumpDiff = maxJumpHeight - startingJumpHeight;
        
        float jumpCharge = (jumpHeight - startingJumpHeight) / jumpDiff;

        speedReward = jumpCharge;

        if (speedReward < 0.25f)
        {
            speedReward = 0.25f;
        }
        else
        {
            speedReward += extraSpeed;
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
        if (landedOnGround)
        {
            turbulenceGenerator.ResumeGeneration(transform.position);
        }

        if (!characterStats.IsPlayer)
        {
            characterStats.Air += 100;
            CanDoTricks = false;
            transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
            playerAnimation.Anim.SetBool("DoingTricks", false);
            return;
        }

        audioHolder.SfxManager.Play(Constants.SoundEffects.land);

        int gainedAir = 0;

        if (!playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") && !CanLand && landedOnGround)
        {
            audioHolder.VoiceManager.Play(Constants.VoiceSounds.landFail);

            if (!characterStats.BoardStats.RingsAsAir)
            {
                gainedAir = 25;
                characterStats.Air += 25;
            }

            tricks = 0;
        }
        else
        {
            if (tricks > 5)
            {
                tricks = 5;
            }
            else if (tricks == 2)
            {
                //2 tricks still give a B rank
                tricks = 1;
            }

            if (!characterStats.BoardStats.RingsAsAir)
            {
                gainedAir = tricks * 25 + 25;
                characterStats.Air += gainedAir;
            }

            if (tricks >= 1)
            {
                audioHolder.VoiceManager.Play(Constants.VoiceSounds.landSucces);                

                float speedMultiplier = ((float)tricks + 7) * 0.1f;

                Debug.Log("Speed multiplier: " + speedMultiplier);

                float speed = characterStats.GetCurrentLimit() * speedMultiplier;

                playerMovement.Speed = speed;
            }  
            else
            {
                audioHolder.VoiceManager.Play(Constants.VoiceSounds.landFail);
            }
        }

        if (tricks > 2)
        {
            tricks--;
        }

        if (characterStats.IsPlayer)
        {
            characterStats.Hud.ShowRank(tricks, gainedAir);
        }

        if (playerMovement.OnWater && playerMovement.Speed < 20)
        {
            playerMovement.Speed = 30;
        }

        tricks = 0;

        transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
        playerAnimation.Anim.SetBool("DoingTricks", false);

        StopTrickInput();

        playerAnimation.Anim.SetFloat("TrickSpeed", 0);
        playerAnimation.Anim.Play("Idle");

        characterStats.Cam.localPosition = characterStats.CamStartPos;
        characterStats.Cam.localRotation = new Quaternion(0, 0, 0, characterStats.Cam.localRotation.w);

    }

    public void StopTrickInput()
    {
        CanDoTricks = false;
        TrickDirection = Vector2.zero;
        playerAnimation.Anim.SetFloat("TrickVerticalDir", 0);
        playerAnimation.Anim.SetFloat("TrickHorizontalDir", 0);
    }
}
