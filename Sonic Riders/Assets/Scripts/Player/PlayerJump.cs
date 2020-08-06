using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerSound playerSound;
    [SerializeField] private SphereCollider groundCol;

    [SerializeField] private float startingJumpHeight = 20;
    [SerializeField] private float jumpHeight = 20;
    [SerializeField] private float maxJumpHeight = 60;
    [SerializeField] private float jumpGain = 1;
    [SerializeField] private float raycastJumpLength = 0.5f;
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }

    [SerializeField] private bool jumpRelease = false;
    [SerializeField] private bool actualJumpRelease = false;
    public bool JumpRelease { get { return jumpRelease; } set { jumpRelease = value; } }
    public bool JumpHold { get; set; } = false;
    public bool JumpHoldControls { get; set; } = false;
    public bool JumpButtonUp { get; set; } = false;
    public bool DontDragDown { get; set; } = false;

    private Rigidbody rb;
    private PlayerMovement mov;
    private PlayerTricks playerTricks;
    private CharacterStats charStats;
    [SerializeField] private float timeForLength = 0.5f;

    private float rampPower;
    public float RampPower { get { return rampPower; } }
    private float maxRampPower;
    private float worstRampPower;

    private bool alreadyFell = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<PlayerMovement>();
        playerTricks = GetComponent<PlayerTricks>();
        charStats = GetComponent<CharacterStats>();
        playerSound = GetComponentInChildren<PlayerSound>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mov.Grounded)
        {
            if (jumpHeight != startingJumpHeight)
            {
                jumpHeight = startingJumpHeight;
            }

            JumpHold = false;

            return;
        }

        if (JumpButtonUp)
        {
            JumpHold = false;            

            if (transform.parent != null)
            {
                Ramp ramp = transform.GetComponentInParent<Ramp>();
                rampPower = ramp.Power;
                maxRampPower = rampPower;
                worstRampPower = ramp.WorstPower;

                if (transform.localPosition.z < ramp.PerfectJump)
                {
                    rampPower -= Mathf.Abs(transform.localPosition.z) * 0.25f;

                    if (transform.localPosition.y < 0)
                    {
                        rampPower -= ramp.PerfectJump / 2;
                    }

                    playerSound.PlaySoundEffect(PlayerSound.sounds.JUMPRAMP);
                }
                else
                {
                    if (!playerTricks.CanDoTricks)
                    {
                        playerSound.PlaySoundEffect(PlayerSound.sounds.PERFECTJUMP);
                    }                    
                }

                Debug.Log("Ramp power " + rampPower);

                mov.Speed = ramp.Speed;
            }
            else
            {
                rampPower = 0;
            }

            jumpRelease = true;
        }        

        if (JumpHoldControls)
        {
            JumpHold = true;
            if (charStats.Air > 0)
            {
                charStats.Air -= 0.05f;                
            }

            if (jumpHeight < maxJumpHeight)
            {
                jumpHeight += jumpGain * Time.deltaTime;
            }
        }        
    }

    private void FixedUpdate()
    {
        if (jumpRelease)
        {
            mov.RaycastLength = raycastJumpLength;
            DontDragDown = true;
            Invoke("CanDragDown", 0.2f);

            if (rampPower > 0)
            {
                Quaternion rot = transform.GetChild(0).rotation;
                rot.y = transform.parent.rotation.y;
                transform.GetChild(0).rotation = rot;

                rb.velocity = transform.GetChild(0).forward * mov.Speed;

                rb.AddForce(transform.parent.GetChild(0).forward * (jumpHeight + rampPower), ForceMode.Impulse);
                transform.parent = null;
                alreadyFell = false;
                playerTricks.ChangeTrickSpeed(rampPower, maxRampPower, worstRampPower, jumpHeight, startingJumpHeight, maxJumpHeight);
            }
            else
            {                
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);                
            }

            jumpHeight = startingJumpHeight;
            jumpRelease = false;
        }
    }

    private void CanDragDown()
    {
        DontDragDown = false;
    }

    public void FallingOffRamp(float worstPower, float speed, float perfectJump, float powerOfRamp)
    {
        if (transform.parent != null && !alreadyFell && transform.localPosition.z > perfectJump && !playerTricks.CanDoTricks)
        {            
            alreadyFell = true;
            rampPower = worstPower;
            maxRampPower = powerOfRamp;
            worstRampPower = worstPower;

            mov.Speed = speed;

            rb.velocity = transform.GetChild(0).forward * speed;

            jumpRelease = true;

            //For now!!!!!
            if (mov.IsPlayer)
            {
                playerSound.PlaySoundEffect(PlayerSound.sounds.JUMPRAMP);
            }           

            Debug.Log("Fell of ramp");
        }
        else
        {
            if (!jumpRelease && !DontDragDown)
            {
                transform.parent = null;
            }
        }
    }
}
