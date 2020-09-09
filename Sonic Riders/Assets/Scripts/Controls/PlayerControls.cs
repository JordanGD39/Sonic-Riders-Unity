using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerControls : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private InputMaster inputMaster;
    private PlayerInput playerInput;
    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerBoost playerBoost;
    private PlayerDrift playerDrift;
    private PlayerJump playerJump;
    private PlayerTricks playerTricks;
    private PlayerFlight playerFlight;
    private PlayerGrind playerGrind;

    private InputAction moveAction;
    private InputAction jumpAction;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        player = GameManager.instance.PlayersLeft[0];
        GameManager.instance.PlayersLeft.Remove(player);

        playerInputManager = GameManager.instance.GetComponent<PlayerInputManager>();

        List<Camera> cams = GameManager.instance.Cams;

        if (playerInputManager.playerCount > 1)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                cams[i].GetComponent<CameraCollision>().MaxDistance = 3.5f;
            }
        }

        switch (playerInputManager.playerCount)
        {
            case 2:
                cams[0].rect = new Rect(0, 0.5f, 1, 0.5f);
                cams[1].rect = new Rect(0, 0, 1, 0.5f);
                break;
            case 3:
                cams[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                cams[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cams[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                break;
            case 4:
                cams[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                cams[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cams[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                cams[3].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                break;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        playerBoost = player.GetComponent<PlayerBoost>();
        playerDrift = player.GetComponent<PlayerDrift>();
        playerGrind = player.GetComponent<PlayerGrind>();
        playerJump = player.GetComponent<PlayerJump>();
        playerTricks = player.GetComponent<PlayerTricks>();
        playerFlight = player.GetComponent<PlayerFlight>();

        inputMaster = new InputMaster();
        
        playerInput.actions.FindAction(inputMaster.Player.Boost.id).performed += ctx => playerBoost.CheckBoost();

        InputAction driftAction = playerInput.actions.FindAction(inputMaster.Player.Drift.id);
        moveAction = playerInput.actions.FindAction(inputMaster.Player.Movement.id);
        jumpAction = playerInput.actions.FindAction(inputMaster.Player.JumpHold.id);

        driftAction.performed += ctx => playerDrift.DriftPressed = true;
        driftAction.canceled += ctx => playerDrift.DriftPressed = true;
        driftAction.canceled += ctx => playerDrift.DriftPressed = false;
        playerInput.actions.FindAction(inputMaster.Player.Grind.id).performed += ctx => CheckGrindJump();
        jumpAction.canceled += ctx => playerJump.CheckRelease();

        //inputMaster.Player.Enable();

        playerMovement.IsPlayer = true;
        playerMovement.CheckIfPlayer();
        GameManager.instance.GetAudioManager.Play("Test");
    }

    // Update is called once per frame
    void Update()
    {
        OnMove(moveAction.ReadValue<Vector2>());

        if (playerMovement.Grounded)
        {
            playerJump.JumpHoldControls = jumpAction.triggered;
        }
        else
        {
            playerJump.JumpHoldControls = false;
        }

        /*if (inputAction == null)
        {
            return;
        }

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

        if (playerGrind.enabled)
        {
            playerGrind.JumpPressed = Input.GetButtonDown("Jump");
        }

        if (playerFlight.enabled)
        {
            playerFlight.VerticalRotation = Input.GetAxis("Vertical");
        }

        playerJump.JumpHoldControls = Input.GetButton("Jump");
        playerJump.JumpButtonUp = Input.GetButtonUp("Jump");*/
    }

    private void OnMove(Vector2 mov)
    {
        playerMovement.Movement = new Vector3(0, 0, mov.y);

        float turnDir = mov.x + playerDrift.DriftDir;

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

        if (playerTricks.CanDoTricks)
        {
            playerTricks.TrickDirection = mov;
        }
        else if (playerFlight.Flying)
        {
            playerFlight.VerticalRotation = mov.y;
        }
    }

    private void CheckGrindJump()
    {
        if (playerGrind.enabled)
        {
            playerGrind.CheckGrind();
        }
    }

    /*public void Drift(InputAction.CallbackContext ctx)
    {
        playerDrift.DriftPressed = ctx.performed;
    }

    public void JumpHold(InputAction.CallbackContext ctx)
    {
        playerJump.JumpHoldControls = ctx.performed;
        playerJump.JumpRelease = ctx.canceled;
    }*/
}
