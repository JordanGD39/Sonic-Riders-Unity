using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
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
    private Text playerText;
    private ButtonSounds buttonSounds;

    private bool selectingBoard = false;
    private GameObject currPrefabInstance;
    private GameObject currPrefab;
    private CharacterStats charStats;
    private PlayerSelectReferences playerSelect;

    [SerializeField] private BoardStats[] boards;
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

        playerSelect = canvas.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(playerInput.playerIndex).GetComponent<PlayerSelectReferences>();

        playerSelect.BoardImage.transform.parent.gameObject.SetActive(false);

        GameObject textPlayer = Instantiate(playerTextPref, button.transform, false);

        playerText = textPlayer.GetComponent<Text>();

        playerIndex = playerInput.playerIndex;

        switch (playerIndex)
        {
            case 0:
                playerText.color = new Color32(245, 47, 46, 255);
                playerText.GetComponent<Outline>().effectColor = new Color32(134, 0, 0, 255);
                playerText.text = "P1";
                break;
            case 1:
                playerText.color = new Color32(84, 98, 255, 255);
                playerText.GetComponent<Outline>().effectColor = Color.blue;
                playerText.text = "P2";
                break;
            case 2:
                playerText.color = new Color32(255, 199, 24, 255);
                playerText.GetComponent<Outline>().effectColor = new Color32(95, 96, 3, 255);
                playerText.text = "P3";
                break;
            case 3:
                playerText.color = new Color32(30, 158, 63, 255);
                playerText.GetComponent<Outline>().effectColor = new Color32(0, 83, 0, 255);
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
        playerSelect.BoardImage.transform.parent.gameObject.SetActive(false);
        playerSelect.BoardImageFinal.gameObject.SetActive(false);
        playerSelect.PointersParent.SetActive(false);
        playerSelect.InfoPanel.SetActive(false);
        eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().RemovePlayer(playerIndex);
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
                playerSelect.InfoText.text = boards[boardIndex].BoardDescription;
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
        }
        else if (selectingBoard)
        {
            RectTransform boardTransform = playerSelect.BoardImage.GetComponent<RectTransform>();
            boardTransform.localPosition = new Vector2(-45, -10);
            boardTransform.localRotation = new Quaternion(0, 0, 0.27f, 0);
            playerSelect.CharacterImage.gameObject.SetActive(true);
            playerSelect.PointersParent.SetActive(false);
            playerSelect.BoardText.gameObject.SetActive(false);
            playerSelect.BoardImage.gameObject.SetActive(false);
            playerSelect.BoardImageFinal.sprite = playerSelect.BoardImage.sprite;
            playerSelect.BoardImageFinal.gameObject.SetActive(true);
            playerReady = true;

            //Disables the panel when opened
            if (playerSelect.InfoPanel.activeSelf)
            {
                ShowInfo();
            }

            PlayerDone();
        }
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
            boardIndex = boards.Length - 1;
        }
        else if (boardIndex > boards.Length - 1)
        {
            boardIndex = 0;
        }
    }

    private void CheckBoard(int index)
    {
        if (boards[index].CharacterRestriction.Count > 0 && !boards[index].CharacterRestriction.Contains(currPrefab))
        {
            if (index > prevBoardIndex)
            {
                index++;
            }
            else
            {
                index--;
            }
        }

        boardIndex = index;

        CheckOutOfRange();

        if (boards[index].IsStandard)
        {
            playerSelect.BoardImage.sprite = charStats.BoardImage;
            playerSelect.BoardText.text = charStats.BoardName;
        }
        else
        {
            playerSelect.BoardImage.sprite = boards[index].BoardIcon;
            playerSelect.BoardText.text = boards[index].BoardName;
        }

        charStats.BoardStats = boards[index];
        characterButton.ShowSelectedCharacter(playerIndex, true, charStats);

        prevBoardIndex = boardIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.instance.TrackToLoad = "Island";
        }

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
        currPrefabInstance = Instantiate(prefab);
        currPrefabInstance.SetActive(false);
        charStats = currPrefabInstance.GetComponent<CharacterStats>();
        playerText.gameObject.SetActive(false);
        playerSelect.CharacterImage.gameObject.SetActive(false);
        playerSelect.BoardImage.transform.parent.gameObject.SetActive(true);
        playerSelect.BoardText.gameObject.SetActive(true);
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
                Destroy(currPrefabInstance);
                prevButton.GetComponent<CharacterButton>().DisabledImage.SetActive(false);
                playerText.gameObject.SetActive(true);
                playerSelect.BoardImage.transform.parent.gameObject.SetActive(false);
                playerSelect.CharacterImage.gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(prevButton);
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().ShowSelectedCharacter(playerIndex, false, null);
                playerConfigManager.PlayerReady(transform.GetSiblingIndex(), false);
                playerSelect.BoardText.gameObject.SetActive(false);
                selectingBoard = false;
            }
            else if(playerReady & !selectingBoard)
            {
                playerSelect.BoardImageFinal.gameObject.SetActive(false);
                playerSelect.BoardText.gameObject.SetActive(true);
            }

            playerSelect.PointersParent.SetActive(true);

            playerReady = false;
        }
        else
        {
            GameManager.instance.LoadScene("MainMenu", false);
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
