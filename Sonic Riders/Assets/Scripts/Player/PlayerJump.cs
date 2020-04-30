using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float startingJumpheight = 20;
    [SerializeField] private float jumpheight = 20;
    [SerializeField] private float maxJumpheight = 60;
    [SerializeField] private float jumpGain = 1;
    
    private bool jumpRelease = false;
    public bool JumpHold { get; set; } = false;

    private Rigidbody rb;
    private PlayerMovement mov;
    private CharacterStats charStats;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<PlayerMovement>();
        charStats = GetComponent<CharacterStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Jump"))
        {
            JumpHold = false;
            jumpRelease = true;
        }

        if (!mov.Grounded)
        {
            if (jumpheight != startingJumpheight)
            {
                jumpheight = startingJumpheight;
            }
            
            return;
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
                jumpheight += jumpGain;
            }
        }        
    }

    private void FixedUpdate()
    {
        if (jumpRelease)
        {
            rb.AddForce(transform.up * jumpheight, ForceMode.Impulse);
            jumpheight = startingJumpheight;
            jumpRelease = false;
        }
    }
}
