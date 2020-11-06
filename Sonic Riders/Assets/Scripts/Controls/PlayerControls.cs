using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private InputMaster inputMaster;
    private InputAction boostAction;
    private PlayerInput playerInput;
    private GameObject player;
    public GameObject Player { get { return player; } }
    private PlayerMovement playerMovement;
    private PlayerBoost playerBoost;
    private PlayerDrift playerDrift;
    private PlayerJump playerJump;
    private PlayerTricks playerTricks;
    private PlayerFlight playerFlight;
    private CharacterStats charStats;
    private PlayerGrind playerGrind;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction grindAction;
    private InputAction driftAction;
    private BigCanvasUI bigCanvasUI;
    private InputAction pauseAction;

    private bool restartedScene = false;

    private void Start()
    {
        if (GameManager.instance.GetComponent<TestHandleJoin>() != null)
        {
            FindPlayer();
        }
    }

    public void FindPlayer()
    {
        playerInput = GetComponent<PlayerInput>();

        player = GameManager.instance.PlayersLeft[0];

        player.GetComponent<AiControls>().enabled = false;

        GameManager.instance.PlayersLeft.Remove(player);

        playerInputManager = GameManager.instance.GetComponent<PlayerInputManager>();

        List<Camera> cams = GameManager.instance.Cams;

        if (playerInputManager.playerCount > 1)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                if (cams[i].GetComponent<CameraCollision>() != null)
                {
                    cams[i].GetComponent<CameraCollision>().MaxDistance = 3.5f;
                }
            }
        }

        bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();

        playerMovement = player.GetComponent<PlayerMovement>();
        playerBoost = player.GetComponent<PlayerBoost>();
        playerDrift = player.GetComponent<PlayerDrift>();
        playerGrind = player.GetComponent<PlayerGrind>();
        playerJump = player.GetComponent<PlayerJump>();
        playerTricks = player.GetComponent<PlayerTricks>();
        playerFlight = player.GetComponent<PlayerFlight>();
        charStats = player.GetComponent<CharacterStats>();

        int playerIndex = transform.GetSiblingIndex();

        if (GameManager.instance.GetComponent<TestHandleJoin>() != null)
        {
            playerIndex = playerInput.playerIndex;
        }


        if (GameManager.instance.PlayersNames.Contains(charStats.CharacterName))
        {
            Material mat = new Material(charStats.PlayerMeshRenderer.material);
            mat.color = Color.gray;
            charStats.PlayerMeshRenderer.material = mat;

            Color darkerColor = new Color(charStats.CharColor.r * 0.001f, charStats.CharColor.g * 0.001f, charStats.CharColor.b * 0.001f, charStats.CharColor.a);

            charStats.CharColor = darkerColor;

            if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
            {
                if (playerInputManager.playerCount >= 3)
                {
                    FindObjectOfType<BigCanvasUI>().ChangeSurvivalColor(playerIndex, darkerColor);
                }
                else
                {
                    HUD[] huds = FindObjectsOfType<HUD>();

                    for (int i = 0; i < huds.Length; i++)
                    {
                        huds[i].ChangeSurvivalColor(playerIndex, darkerColor);
                    }
                }
            }            
        }

        GameManager.instance.PlayersNames.Add(charStats.CharacterName);

        charStats.IsPlayer = true;

        Transform canvasHolder = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform;
        
        charStats.Canvas = canvasHolder.transform.GetChild(playerIndex);

        playerMovement.GiveCanvasHud();
        playerGrind.GiveCanvasHud();
        playerFlight.GiveCanvasHud();
        charStats.GiveCanvasHud();
        playerDrift.GiveAnim();
        playerBoost.GiveAnim();
        player.GetComponent<PlayerCheckpoints>().GiveHud(charStats.Canvas.GetComponent<HUD>());        

        if (playerInputManager.playerCount > 1)
        {
            if (playerInputManager.playerCount == 2)
            {
                cams[0].rect = new Rect(0, 0.5f, 1, 0.5f);
                cams[1].rect = new Rect(0, 0, 1, 0.5f);
            }
            else if (playerInputManager.playerCount > 2)
            {
                cams[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                cams[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cams[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                cams[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
            }

            for (int i = 0; i < playerInputManager.playerCount; i++)
            {
                CanvasScaler scaler = canvasHolder.GetChild(i).GetComponent<CanvasScaler>();
                scaler.referenceResolution = new Vector2(1600, 1200);
            }
        }       

        if (!restartedScene)
        {
            AddActions();
        }

        ControlsEnable();

        restartedScene = true;
        //inputMaster.Player.Enable();
        
        charStats.PlayerIndex = playerIndex;
        
        charStats.Cam = cams[playerIndex].transform.parent;
        charStats.CamStartPos = charStats.Cam.localPosition;
        charStats.Cam.GetComponentInChildren<CameraDeath>().GiveCanvasAnim();

        if (!GameManager.instance.TestAir)
        {
            charStats.Air = 0;
            charStats.GetComponent<PlayerAnimationHandler>().Anim.Play("Standing");
        }

        if (!charStats.BoardStats.RingsAsAir)
        {
            charStats.Canvas.GetComponent<HUD>().UpdateAirBar(charStats.Air, charStats.MaxAir);
        }

        //GameManager.instance.GetAudioManager.Play("Test");
    }

    public void ControlsEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        driftAction.Enable();
        boostAction.Enable();
        grindAction.Enable();
    }

    public void ControlsDisable()
    {
        if (moveAction == null)
        {
            return;
        }

        moveAction.Disable();
        jumpAction.Disable();
        driftAction.Disable();
        boostAction.Disable();
        grindAction.Disable();
    }

    private void AddActions()
    {
        inputMaster = new InputMaster();

        boostAction = playerInput.actions.FindAction(inputMaster.Player.Boost.id);
        driftAction = playerInput.actions.FindAction(inputMaster.Player.Drift.id);
        moveAction = playerInput.actions.FindAction(inputMaster.Player.Movement.id);
        jumpAction = playerInput.actions.FindAction(inputMaster.Player.JumpHold.id);
        grindAction = playerInput.actions.FindAction(inputMaster.Player.Grind.id);

        boostAction.performed += ctx => playerBoost.CheckBoost();
        driftAction.performed += ctx => playerDrift.DriftPressed = true;
        driftAction.canceled += ctx => playerDrift.DriftPressed = true;
        driftAction.canceled += ctx => playerDrift.DriftPressed = false;
        grindAction.performed += ctx => CheckGrindJump();
        jumpAction.performed += ctx => playerJump.JumpHoldControls = true;
        jumpAction.canceled += ctx => playerJump.JumpHoldControls = false;
        jumpAction.canceled += ctx => playerJump.CheckRelease();       

        pauseAction = playerInput.actions.FindAction(inputMaster.Player.Pause.id);

        pauseAction.performed += ctx => CheckBigCanvas();
    }

    private void CheckBigCanvas()
    {
        if (bigCanvasUI != null)
        {
            bigCanvasUI.PauseToggle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction == null || charStats == null)
        {
            return;
        }
        //Debug.Log(moveAction.ReadValue<Vector2>());

        if (!charStats.DisableAllFeatures)
        {
            OnMove(moveAction.ReadValue<Vector2>());
        }
        else
        {
            OnMove(new Vector2(0, 0));
        }

        /*if (playerMovement.Grounded)
        {
            playerJump.JumpHoldControls = jumpAction.triggered;
        }
        else
        {
            playerJump.JumpHoldControls = false;
        }*/

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
        playerMovement.Movement = new Vector3(mov.x, 0, mov.y);

        float turnDir = mov.x + playerDrift.DriftDir;
        float driftRate = 1.5f; 

        if (charStats.BoardStats.AutoDrift)
        {
            driftRate = 1.2f;
        }

        if (playerDrift.DriftPressed && playerMovement.Grounded)
        {
            if (Mathf.Abs(turnDir) < 0.2f)
            {
                turnDir = 0.2f * playerDrift.DriftDir;
            }
            else if (Mathf.Abs(turnDir) > 1.5f)
            {
                turnDir = driftRate * playerDrift.DriftDir;
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
