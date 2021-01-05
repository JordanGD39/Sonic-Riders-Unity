using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private AudioManagerHolder audioHolder;

    [SerializeField] private float startingJumpHeight = 20;
    [SerializeField] private float jumpHeight = 20;
    [SerializeField] private float maxJumpHeight = 60;
    [SerializeField] private float jumpGain = 1;
    [SerializeField] private float raycastJumpLength = 0.5f;
    private float jumpMultiplier = 1;
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }
    public float GrindJumpHeight { get; set; } = 0;

    [SerializeField] private bool jumpRelease = false;
    public bool JumpRelease { get { return jumpRelease; } set { jumpRelease = value; } }
    public bool JumpHold { get; set; } = false;
    public bool JumpHoldControls { get; set; } = false;
    public bool DontDragDown { get; set; } = false;

    private Rigidbody rb;
    private PlayerMovement mov;
    private PlayerTricks playerTricks;
    private CharacterStats charStats;
    //[SerializeField] private float timeForLength = 0.5f;

    [SerializeField] private float rampPower;
    public float RampPower { get { return rampPower; } set { rampPower = value; } }
    //private float maxRampPower;
    //private float worstRampPower;

    [SerializeField] private float highestYvel = 0;

    public Ramp CurrRamp { get; set; }

    private bool alreadyFell = false;
    private bool canClamp = false;

    // Start is called before the first frame update
    void Start()
    {
        float gravMultiplier = GameManager.instance.GravitityMultiplier;
        startingJumpHeight *= gravMultiplier;
        jumpHeight *= gravMultiplier;
        maxJumpHeight *= gravMultiplier;
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<PlayerMovement>();
        playerTricks = GetComponent<PlayerTricks>();
        charStats = GetComponent<CharacterStats>();
        audioHolder = GetComponent<AudioManagerHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mov.Grounded || charStats.Air <= 0 || charStats.DisableAllFeatures)
        {
            if (jumpHeight != startingJumpHeight)
            {
                jumpHeight = startingJumpHeight;
            }

            JumpHold = false;

            return;
        }

        if (JumpHoldControls)
        {
            HoldingJump();
        }
    }

    public void HoldingJump()
    {
        if (charStats.SuperForm)
        {
            charStats.ResetSuperRotation();
        }

        JumpHold = true;

        if (charStats.Air > 0)
        {
            charStats.Air -= charStats.GetCurrentAirLoss() * 4 * Time.deltaTime;
        }

        if (jumpHeight < maxJumpHeight)
        {
            jumpHeight += jumpGain * Time.deltaTime;
        }
    }

    public void CheckRelease()
    {
        if (mov.Grounded && charStats.Air > 0 && !charStats.DisableAllFeatures)
        {
            JumpHold = false;

            if (transform.parent != null)
            {
                CurrRamp = transform.GetComponentInParent<Ramp>();

                if (CurrRamp.Flight)
                {
                    rampPower = 0;
                    jumpRelease = true;
                    return;
                }

                rampPower = CurrRamp.Power;
                //maxRampPower = rampPower;

                jumpMultiplier = CurrRamp.JumpMultiplier;

                if (!playerTricks.CanDoTricks)
                {
                    audioHolder.VoiceManager.Play(Constants.VoiceSounds.rampJump);
                }

                Debug.Log("Ramp power " + rampPower);
            }
            else
            {
                rampPower = 0;
            }

            jumpRelease = true;
        }
    }

    public void StartCanDragDown()
    {
        DontDragDown = true;
        Invoke("CanDragDown", 0.5f);
    }

    private void FixedUpdate()
    {
        if (jumpRelease)
        {
            audioHolder.SfxManager.Play(Constants.SoundEffects.jump);
            
            StartCanDragDown();
            mov.RaycastLength = raycastJumpLength;

            if (rampPower > 0 && transform.parent != null)
            {
                canClamp = false;

                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

                Vector3 forward = transform.parent.GetChild(0).forward;

                if (CurrRamp.DifferentForward != null)
                {
                    Transform angleTransform = transform.parent.GetChild(0);

                    Quaternion oldRot = angleTransform.localRotation;

                    angleTransform.forward = transform.GetChild(0).forward;

                    angleTransform.localRotation = new Quaternion(oldRot.x, angleTransform.localRotation.y, oldRot.z, oldRot.w);

                    transform.GetChild(0).forward = CurrRamp.DifferentForward.forward;
                    transform.GetChild(0).localRotation = new Quaternion(0, transform.GetChild(0).localRotation.y, 0, transform.GetChild(0).localRotation.w);

                    transform.GetChild(0).GetChild(0).localRotation = new Quaternion(0, 0, CurrRamp.JumpRotationZ, transform.GetChild(0).GetChild(0).localRotation.w);

                    //Debug.Log(transform.GetChild(0).GetChild(0).localRotation);
                    //transform.up = CurrRamp.DifferentForward.up;
                } 
                else
                {
                    Quaternion rot = new Quaternion(0, 0, 0, 0);
                    transform.GetChild(0).forward = transform.parent.forward;
                    rot.y = transform.GetChild(0).localRotation.y;
                    rot.w = transform.GetChild(0).localRotation.w;
                    transform.GetChild(0).localRotation = rot;
                }

                //rb.velocity = transform.GetChild(0).forward * mov.Speed;

                float jumpPower = jumpHeight;

                if (charStats.BoardStats.AutoTrick)
                {
                    jumpPower = maxJumpHeight;
                }
                
                transform.SetParent(null);
                rb.velocity = forward * (jumpPower * jumpMultiplier + rampPower);

                alreadyFell = false;
                playerTricks.ChangeTrickSpeed(jumpPower, startingJumpHeight, maxJumpHeight);
            }
            else
            {
                if (jumpHeight < startingJumpHeight + 1)
                {
                    charStats.Air -= 1;
                }
                audioHolder.VoiceManager.Play(Constants.VoiceSounds.jump);

                rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
                highestYvel = 0;
                canClamp = true;
            }

            jumpHeight = startingJumpHeight;
            GrindJumpHeight = 0;
            jumpRelease = false;
        }

        if (rb.velocity.y > highestYvel)
        {
            highestYvel = rb.velocity.y;
        }

        if (canClamp && !mov.Grounded)
        {
            Vector3 localVel = transform.GetChild(0).InverseTransformDirection(rb.velocity);
            localVel.y = Mathf.Clamp(localVel.y, -99, 6.5f + (jumpHeight - startingJumpHeight));
            rb.velocity = transform.GetChild(0).TransformDirection(localVel);
            //Debug.Log(localVel);
        }
    }

    private void CanDragDown()
    {
        DontDragDown = false;
    }

    public void FallingOffRamp(Ramp ramp)
    {
        if (transform.parent != null && !alreadyFell && transform.localPosition.z > ramp.PerfectJump && !playerTricks.CanDoTricks)
        {            
            alreadyFell = true;
            CurrRamp = ramp;

            if (charStats.BoardStats.AutoTrick)
            {
                rampPower = ramp.Power;
            }
            else
            {
                rampPower = ramp.WorstPower;
            }

            //maxRampPower = ramp.Power;
            //worstRampPower = rampPower;
            jumpMultiplier = ramp.JumpMultiplier;

            Debug.Log("Ramp power " + rampPower);

            jumpRelease = true;

            audioHolder.VoiceManager.Play(Constants.VoiceSounds.rampJump);                 
        }
        else
        {
            if (!jumpRelease)
            {
                transform.parent = null;
            }
        }
    }
}
