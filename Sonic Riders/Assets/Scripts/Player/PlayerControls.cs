using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerBoost playerBoost;
    private PlayerDrift playerDrift;
    private PlayerJump playerJump;
    private PlayerTricks playerTricks;
    private PlayerGrind playerGrind;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();    
        playerBoost = GetComponent<PlayerBoost>();  
        playerDrift = GetComponent<PlayerDrift>();
        playerGrind = GetComponent<PlayerGrind>();
        playerJump = GetComponent<PlayerJump>();
        playerTricks = GetComponent<PlayerTricks>();
        playerMovement.IsPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.Movement = new Vector3(0, 0, Input.GetAxis("Vertical"));

        float turnDir = Input.GetAxis("Horizontal") + playerDrift.DriftDir;

        if (playerDrift.DriftPressed && playerMovement.Grounded)
        {
            if (Mathf.Abs(turnDir) < 0.2f)
            {
                turnDir = 0.2f * playerDrift.DriftDir;
            }
            else if (Mathf.Abs(turnDir) > 1.5f)
            {
                turnDir = 1.5f * playerDrift.DriftDir;
            }
        }

        playerMovement.TurnAmount = turnDir;

        playerTricks.TrickDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        playerBoost.BoostPressed =  Input.GetButtonDown("Boost");

        playerDrift.DriftPressed = Input.GetButton("Drift");

        playerGrind.JumpPressed = Input.GetButtonDown("Jump");

        playerJump.JumpHoldControls = Input.GetButton("Jump");
        playerJump.JumpButtonUp = Input.GetButtonUp("Jump");
    }
}
