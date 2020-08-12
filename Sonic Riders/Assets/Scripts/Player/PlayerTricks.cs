using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTricks : MonoBehaviour
{
    public bool CanDoTricks { get; set; } = false;
    public Vector2 TrickDirection { get; set; }

    private PlayerMovement playerMovement;
    private PlayerSound playerSound;
    private PlayerAnimationHandler playerAnimation;
    private CharacterStats characterStats;

    private float speedReward;
    [SerializeField] private int tricks = 0;
    public bool CanLand { get; set; } = false;

    private Transform cam;
    private Vector3 camStartingPos;
    [SerializeField] private float camSpeed = 1;
    private Rigidbody rb;
    [SerializeField] private Vector3 lowerCamPos;
    [SerializeField] private Vector3 higherCamPos;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform.parent;
        camStartingPos = cam.localPosition;

        playerMovement = GetComponent<PlayerMovement>();
        playerSound = GetComponentInChildren<PlayerSound>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        characterStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (CanDoTricks && playerMovement.IsPlayer)
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
            cam.localPosition = Vector3.MoveTowards(cam.localPosition, pos, step);
            cam.LookAt(transform.position);
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
        rampTiming *= 0.7f;
        jumpCharge *= 0.3f;

        speedReward = rampTiming + jumpCharge;

        if (speedReward < 0.25f)
        {
            speedReward = 0.25f;
        }

        if (playerMovement.IsPlayer)
        {
            playerAnimation.Anim.SetFloat("TrickSpeed", speedReward);
        }
        
        Debug.Log("Trick speed: " + speedReward);
        transform.GetChild(0).rotation = new Quaternion(0, transform.GetChild(0).rotation.y, 0, transform.GetChild(0).rotation.w);        
    }

    // Update is called once per frame
    public void Landed()
    {
        if (!playerMovement.IsPlayer)
        {
            characterStats.Air += 50;
            characterStats.Air += 30;
            CanDoTricks = false;
            transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
            return;
        }

        playerSound.PlaySoundEffect(PlayerSound.voiceSounds.NONE, PlayerSound.sounds.LAND);

        if (!playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("Falling") && !CanLand)
        {
            playerSound.PlaySoundEffect(PlayerSound.voiceSounds.RAMPFAIL, PlayerSound.sounds.NONE);
            playerMovement.Speed *= 0.1f;
            Debug.Log("Trick speed loss!!!");
            characterStats.Air -= 15;
        }
        else
        {
            characterStats.Air += speedReward * 50;
            characterStats.Air += tricks * 30;

            if (speedReward > 0.5f)
            {
                playerSound.PlaySoundEffect(PlayerSound.voiceSounds.JUMPSUCCES, PlayerSound.sounds.NONE);
                playerMovement.Speed = characterStats.BoardStats.Limit[characterStats.Level];
            }  
            else
            {
                playerSound.PlaySoundEffect(PlayerSound.voiceSounds.RAMPFAIL, PlayerSound.sounds.NONE);
            }
        }

        tricks = 0;

        transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);
        playerAnimation.Anim.SetBool("DoingTricks", false);

        CanDoTricks = false;

        cam.localPosition = camStartingPos;
        cam.localRotation = new Quaternion(0, 0, 0, cam.localRotation.w);
    }
}
