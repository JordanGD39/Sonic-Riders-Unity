using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerSound playerSound;

    [SerializeField] private float startingJumpheight = 20;
    [SerializeField] private float jumpheight = 20;
    [SerializeField] private float maxJumpheight = 60;
    [SerializeField] private float jumpGain = 1;
    [SerializeField] private float raycastJumpLength = 0.5f;
    public float JumpHeight { set { jumpheight = value; } }

    [SerializeField] private bool jumpRelease = false;
    public bool JumpRelease { get { return jumpRelease; } set { jumpRelease = value; } }
    public bool JumpHold { get; set; } = false;

    private Rigidbody rb;
    private PlayerMovement mov;
    private PlayerTricks playerTricks;
    private CharacterStats charStats;
    [SerializeField] private float timeForLength = 0.5f;

    private float rampPower;

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
            if (jumpheight != startingJumpheight)
            {
                jumpheight = startingJumpheight;
            }

            JumpHold = false;

            return;
        }

        if (Input.GetButtonUp("Jump"))
        {
            JumpHold = false;            

            if (transform.parent != null)
            {
                Ramp ramp = transform.GetComponentInParent<Ramp>();
                rampPower = ramp.Power;

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

        if (Input.GetButton("Jump"))
        {
            JumpHold = true;
            if (charStats.Air > 0)
            {
                charStats.Air -= 0.05f;                
            }

            if (jumpheight < maxJumpheight)
            {
                jumpheight += jumpGain * Time.deltaTime;
            }
        }        
    }

    private void FixedUpdate()
    {
        if (jumpRelease)
        {
            mov.RaycastLength = raycastJumpLength;

            if (rampPower > 0)
            {
                Quaternion rot = transform.GetChild(0).rotation;
                rot.y = transform.parent.rotation.y;
                transform.GetChild(0).rotation = rot;

                rb.velocity = transform.GetChild(0).forward * mov.Speed;

                rb.AddForce(transform.parent.GetChild(0).forward * (jumpheight + rampPower), ForceMode.Impulse);
                transform.parent = null;
                alreadyFell = false;
                playerTricks.CanDoTricks = true;
            }
            else
            {                
                rb.AddForce(transform.up * jumpheight, ForceMode.Impulse);                
            }

            jumpheight = startingJumpheight;
            jumpRelease = false;
        }
    }

    public void FallingOffRamp(float worstPower, float speed, float perfectJump)
    {
        if (transform.parent != null && !alreadyFell && transform.localPosition.z > perfectJump && !playerTricks.CanDoTricks)
        {            
            alreadyFell = true;
            rampPower = worstPower;

            mov.Speed = speed;

            rb.velocity = transform.GetChild(0).forward * speed;

            jumpRelease = true;

            playerSound.PlaySoundEffect(PlayerSound.sounds.JUMPRAMP);

            Debug.Log("Fell of ramp");
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
