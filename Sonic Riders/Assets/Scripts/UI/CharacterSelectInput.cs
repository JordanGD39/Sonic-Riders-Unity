using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectInput : MonoBehaviour
{
    private bool canSelect = false;
    private bool canCancel = false;
    public bool CanSelect { get { return canSelect; } }

    public bool PressedButton { get; set; } = false;

    private PlayerInput playerInput;
    private PlayerConfigManager playerConfigManager;
    private MultiplayerEventSystem eventSystem;
    public MultiplayerEventSystem EventSystem { get { return eventSystem; } }
    private GameObject prevButton;
    [SerializeField] private GameObject playerTextPref;
    private TextMeshProUGUI playerText;
    private ButtonSounds buttonSounds;

    private bool selectingBoard = false;
    private GameObject currPrefabInstance;
    private GameObject currPrefab;
    private CharacterStats charStats;
    private PlayerSelectReferences playerSelect;

    [SerializeField] private BoardStats[] boards;
    [SerializeField] private Material[] playerColorMaterials;
    private List<BoardStats> selectableBoards = new List<BoardStats>();
    private int boardIndex = 0;
    private int prevBoardIndex = 0;
    private CharacterButton characterButton;
    private bool playerReady = false;
    private bool alreadyAddedActions = false;
    private bool leaving = false;

    private int playerIndex = 0;
    public int PlayerIndex { get { return playerIndex; } }

    // Start is called before the first frame update
    private void Start()
    {
        StartFunctions();
    }

    public void StartFunctions()
    {
        if (SceneManager.GetActiveScene().name != "CharacterSelect")
        {
            enabled = false;
            return;
        }

        selectableBoards.Clear();

        prevButton = null;
        playerReady = false;
        selectingBoard = false;

        GameObject canvas = GameObject.FindGameObjectWithTag(Constants.Tags.canvas);

        GameObject button = canvas.GetComponentInChildren<Button>().gameObject;
        characterButton = button.GetComponent<CharacterButton>();

        buttonSounds = canvas.GetComponentInChildren<ButtonSounds>();

        playerConfigManager = GameManager.instance.GetComponent<PlayerConfigManager>();

        if (!alreadyAddedActions)
        {
            alreadyAddedActions = true;
            AddActions();
        }

        eventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button);

        playerSelect = canvas.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(playerInput.playerIndex).GetComponent<PlayerSelectReferences>();

        GameObject textPlayer = Instantiate(playerTextPref, button.transform, false);

        playerText = textPlayer.GetComponent<TextMeshProUGUI>();

        playerIndex = playerInput.playerIndex;

        switch (playerIndex)
        {
            case 1:
                playerText.fontMaterial = playerColorMaterials[0];
                playerText.color = Color.white;

                Color32 colorTop = new Color32(148, 157, 255, 255);
                Color32 colorBottom = new Color32(105, 112, 188, 255);

                playerText.colorGradient = new VertexGradient(colorTop, colorTop, colorBottom, colorBottom);
                playerText.text = "P2";
                break;
            case 2:
                playerText.fontMaterial = playerColorMaterials[1];
                playerText.color = Color.white;

                Color32 colorTopY = new Color32(255, 249, 66, 255);
                Color32 colorBottomY = new Color32(195, 190, 49, 255);

                playerText.colorGradient = new VertexGradient(colorTopY, colorTopY, colorBottomY, colorBottomY);

                playerText.text = "P3";
                break;
            case 3:
                playerText.fontMaterial = playerColorMaterials[2];
                playerText.color = Color.white;

                Color32 colorTopG = new Color32(31, 147, 64, 255);
                Color32 colorBottomG = new Color32(25, 96, 45, 255);

                playerText.colorGradient = new VertexGradient(colorTopG, colorTopG, colorBottomG, colorBottomG);

                playerText.text = "P4";
                break;
        }

        Invoke("CanSelectInput", 0.25f);
        Invoke("CanCancelInput", 0.25f);
    }

    private void AddActions()
    {
        InputMaster inputMaster = new InputMaster();
        playerInput = GetComponent<PlayerInput>();        
        playerInput.actions.FindAction(inputMaster.UI.Movement.id).performed += ctx => ScrollTroughBoard(ctx.ReadValue<Vector2>().x);
        playerInput.actions.FindAction(inputMaster.UI.Cancel.id).performed += ctx => CancelSelection();
        playerInput.actions.FindAction(inputMaster.UI.Select.id).performed += ctx => SelectButton();
        playerInput.actions.FindAction(inputMaster.UI.Leave.id).performed += ctx => PlayerLeft();
        playerInput.actions.FindAction(inputMaster.UI.Leave.id).Enable();

        InputAction helpAction = playerInput.actions.FindAction(inputMaster.UI.Help.id);

        helpAction.performed += ctx => ShowInfo();
        helpAction.Enable();        
    }

    private void PlayerLeft()
    {
        if (leaving)
        {
            return;
        }

        canCancel = false;
        canSelect = false;

        InputMaster inputMaster = new InputMaster();
        playerInput.actions.FindAction(inputMaster.UI.Leave.id).Disable();

        leaving = true;

        ResetPlayerSelect();

        Destroy(eventSystem.gameObject);

        playerConfigManager.RemovePlayer(playerInput.playerIndex);
        playerInput.user.UnpairDevicesAndRemoveUser();

        Destroy(gameObject);
    }

    private void ResetPlayerSelect()
    {
        playerSelect.JoinedPanel.SetActive(false);
        playerSelect.NotJoinedPanel.SetActive(true);
        playerSelect.BoardImage.gameObject.SetActive(false);
        playerSelect.CharacterImage.gameObject.SetActive(true);
        playerSelect.BoardImageFinal.gameObject.SetActive(false);
        playerSelect.BoardText.gameObject.SetActive(false);
        playerSelect.PointersParent.SetActive(false);
        playerSelect.InfoPanel.SetActive(false);
        prevButton.GetComponent<CharacterButton>().RemovePlayer(playerIndex);
        Destroy(playerText.gameObject);
    }

    private void ShowInfo()
    {
        if (playerSelect != null && playerSelect.InfoPanel.activeSelf)
        {
            playerSelect.InfoPanel.SetActive(false);
            return;
        }

        if (eventSystem.currentSelectedGameObject != null)
        {
            playerSelect.InfoText.text = playerSelect.OldInfoText;
        }
        else if (selectingBoard)
        {
            if (boards[boardIndex].IsStandard)
            {
                playerSelect.InfoText.text = "The standard Extreme Gear";
            }
            else
            {
                playerSelect.InfoText.text = selectableBoards[boardIndex].BoardDescription;
            }
        }
        else
        {
            return;
        }

        playerSelect.InfoPanel.SetActive(true);
    }

    private void SelectButton()
    {
        if (!canSelect)
        {
            return;
        }

        buttonSounds.Pressed();            

        if(eventSystem.currentSelectedGameObject != null)
        {
            GameObject prefab = eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().CheckCharacter();

            if (prefab != null)
            {
                ConfirmCharacter(prefab);
            }
            else
            {
                return;
            }

            eventSystem.SetSelectedGameObject(null);
            canSelect = false;            

            Invoke("CanSelectInput", 0.25f);

            if (GameManager.instance.GameMode == GameManager.gamemode.TUTORIAL)
            {
                ShowResult();
            }
        }
        else if (selectingBoard)
        {
            ShowResult();
        }
    }

    private void ShowResult()
    {
        playerSelect.CharacterImage.gameObject.SetActive(true);

        if (charStats.BoardStats.Super)
        {
            playerSelect.CharacterImage.sprite = charStats.SuperSprite;
        }
        else
        {
            playerSelect.BoardImageFinal.sprite = playerSelect.BoardImage.sprite;
            playerSelect.BoardImageFinal.gameObject.SetActive(true);
        }

        playerSelect.PointersParent.SetActive(false);
        playerSelect.BoardText.gameObject.SetActive(false);
        playerSelect.BoardImage.gameObject.SetActive(false);

        playerReady = true;

        //Disables the panel when opened
        if (playerSelect.InfoPanel.activeSelf)
        {
            ShowInfo();
        }

        PlayerDone();
    }

    private void ScrollTroughBoard(float dir)
    {
        int direction = Mathf.RoundToInt(dir);

        if (!selectingBoard || direction == 0 || playerReady)
        {
            return;
        }

        playerSelect.InfoPanel.SetActive(false);

        boardIndex += direction;

        CheckOutOfRange();

        buttonSounds.Select();

        CheckBoard(boardIndex);        
    }

    private void CheckOutOfRange()
    {
        if (boardIndex < 0)
        {
            boardIndex = selectableBoards.Count - 1;
        }
        else if (boardIndex > selectableBoards.Count - 1)
        {
            boardIndex = 0;
        }
    }

    private void CheckBoard(int index)
    {
        boardIndex = index;

        CheckOutOfRange();

        if (boards[index].IsStandard)
        {
            playerSelect.BoardImage.sprite = charStats.BoardImage;
            playerSelect.BoardText.text = charStats.BoardName;
        }
        else
        {
            playerSelect.BoardImage.sprite = selectableBoards[index].BoardIcon;
            playerSelect.BoardText.text = selectableBoards[index].BoardName;
        }

        charStats.BoardStats = selectableBoards[index];
        characterButton.ShowSelectedCharacter(playerIndex, true, charStats);

        prevBoardIndex = boardIndex;
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.instance.TrackToLoad = "Island";
        }*/

        if (eventSystem != null && eventSystem.currentSelectedGameObject != null && !selectingBoard && !leaving)
        {            
            playerText.transform.SetParent(eventSystem.currentSelectedGameObject.transform, false);
            playerText.transform.localPosition = new Vector3(0, 22, 0);

            if (eventSystem.currentSelectedGameObject != prevButton)
            {
                buttonSounds.Select();
                playerSelect.InfoPanel.SetActive(false);
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().ShowSelectedCharacter(playerIndex, false, null);
            }

            prevButton = eventSystem.currentSelectedGameObject;
        }
    }

    public void ConfirmCharacter(GameObject prefab)
    {
        currPrefab = prefab;

        selectableBoards.Clear();
        selectableBoards.AddRange(boards);

        for (int i = 0; i < selectableBoards.Count; i++)
        {
            if (selectableBoards[i].CharacterRestriction.Count > 0 && !selectableBoards[i].CharacterRestriction.Contains(prefab))
            {
                selectableBoards.RemoveAt(i);
                i = 0;
            }
        }

        charStats = gameObject.AddComponent<CharacterStats>();
        charStats.StopCounting = true;
        CharacterStats prefStats = prefab.GetComponent<CharacterStats>();
        charStats.BoardImage = prefStats.BoardImage;
        charStats.SuperSprite = prefStats.SuperSprite;
        charStats.BoardName = prefStats.BoardName;
        charStats.RunSpeed = prefStats.RunSpeed;
        charStats.ExtraSpeed = prefStats.ExtraSpeed;
        charStats.ExtraDash = prefStats.ExtraDash;
        charStats.ExtraPower = prefStats.ExtraPower;
        charStats.ExtraCornering = prefStats.ExtraCornering;

        playerText.gameObject.SetActive(false);
        playerSelect.CharacterImage.gameObject.SetActive(false);
        playerSelect.BoardImage.gameObject.SetActive(true);
        playerSelect.BoardText.gameObject.SetActive(true);
        playerSelect.PointersParent.SetActive(true);

        CheckBoard(0);
        selectingBoard = true;
    }

    private void PlayerDone()
    {
        playerConfigManager.SetPlayerPrefab(transform.GetSiblingIndex(), currPrefab, charStats.BoardStats);
        playerConfigManager.PlayerReady(transform.GetSiblingIndex(), true);
    }

    private void CancelSelection()
    {
        if (!canCancel)
        {
            return;
        }

        canCancel = false;

        buttonSounds.Cancel();

        if (eventSystem.currentSelectedGameObject == null)
        {
            if (!playerReady && selectingBoard)
            {
                Destroy(charStats);                
                playerText.gameObject.SetActive(true);
                playerSelect.BoardImage.gameObject.SetActive(false);
                playerSelect.CharacterImage.gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(prevButton);
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().ShowSelectedCharacter(playerIndex, false, null);
                playerConfigManager.PlayerReady(transform.GetSiblingIndex(), false);
                playerSelect.BoardText.gameObject.SetActive(false);
                playerSelect.PointersParent.SetActive(false);
                selectingBoard = false;
            }
            else if(playerReady & selectingBoard)
            {
                playerSelect.BoardImage.gameObject.SetActive(true);
                playerSelect.BoardText.gameObject.SetActive(true);
                playerSelect.PointersParent.SetActive(true);
                playerSelect.BoardImageFinal.gameObject.SetActive(false);
                playerSelect.CharacterImage.gameObject.SetActive(false);
            }

            playerReady = false;
        }
        else
        {
            GameManager.instance.LoadScene("TrackSelect", false);
        }

        Invoke("CanCancelInput", 0.25f);
    }

    private void CanCancelInput()
    {
        canCancel = true;
    }

    private void CanSelectInput()
    {
        canSelect = true;
    }
}
