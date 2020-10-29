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
    public bool CanSelect { get { return canSelect; } }

    public bool PressedButton { get; set; } = false;

    private PlayerInput playerInput;
    private PlayerConfigManager playerConfigManager;
    private MultiplayerEventSystem eventSystem;
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
    private CharacterButton characterButton;
    private bool playerReady = false;

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

        InputMaster inputMaster = new InputMaster();
        playerInput = GetComponent<PlayerInput>();
        eventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
        playerInput.actions.FindAction(inputMaster.Player.Movement.id).performed += ctx => ScrollTroughBoard(ctx.ReadValue<Vector2>().x);
        playerInput.actions.FindAction(inputMaster.UI.Cancel.id).performed += ctx => CancelSelection();
        playerInput.actions.FindAction(inputMaster.UI.Select.id).performed += ctx => SelectButton();

        playerSelect = canvas.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(playerInput.playerIndex).GetComponent<PlayerSelectReferences>();

        playerSelect.BoardImage.transform.parent.gameObject.SetActive(false);

        GameObject textPlayer = Instantiate(playerTextPref, button.transform, false);

        playerText = textPlayer.GetComponent<Text>();

        switch (playerInput.playerIndex)
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
    }

    private void SelectButton()
    {
        if (!canSelect)
        {
            return;
        }

        if(eventSystem.currentSelectedGameObject != null)
        {
            buttonSounds.Pressed();
            eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().Pressed(playerInput.playerIndex);
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
            playerReady = true;
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

        boardIndex += direction;

        if (boardIndex < 0)
        {
            boardIndex = boards.Length - 1;
        }
        else if (boardIndex > boards.Length - 1)
        {
            boardIndex = 0;
        }

        buttonSounds.Select();

        CheckBoard(boardIndex);       
    }

    private void CheckBoard(int index)
    {
        if (boards[index].IsStandard)
        {
            playerSelect.BoardImage.sprite = charStats.BoardImage;
        }
        else
        {
            playerSelect.BoardImage.sprite = boards[index].BoardIcon;
        }

        charStats.BoardStats = boards[index];
        characterButton.ShowSelectedCharacter(playerInput.playerIndex, true, charStats);
    }

    private void Update()
    {
        if (eventSystem != null && eventSystem.currentSelectedGameObject != null && !selectingBoard)
        {            
            playerText.transform.SetParent(eventSystem.currentSelectedGameObject.transform, false);
            playerText.transform.localPosition = new Vector3(0, 22, 0);

            if (eventSystem.currentSelectedGameObject != prevButton)
            {
                buttonSounds.Select();
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().ShowSelectedCharacter(playerInput.playerIndex, false, null);
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
        CheckBoard(0);
        selectingBoard = true;
    }

    private void PlayerDone()
    {
        playerConfigManager.SetPlayerPrefab(playerInput.playerIndex, currPrefab, charStats.BoardStats);
        playerConfigManager.PlayerReady(playerInput.playerIndex, true);
    }

    private void CancelSelection()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            buttonSounds.Cancel();

            if(!playerReady)
            {
                Destroy(currPrefab);
                prevButton.GetComponent<CharacterButton>().DisabledImage.SetActive(false);
                playerText.gameObject.SetActive(true);
                playerSelect.BoardImage.transform.parent.gameObject.SetActive(false);
                playerSelect.CharacterImage.gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(prevButton);
                eventSystem.currentSelectedGameObject.GetComponent<CharacterButton>().ShowSelectedCharacter(playerInput.playerIndex, false, null);
                playerConfigManager.PlayerReady(playerInput.playerIndex, false);
                selectingBoard = false;
            }

            RectTransform boardTransform = playerSelect.BoardImage.GetComponent<RectTransform>();
            boardTransform.localPosition = new Vector2(-75, -10);
            boardTransform.localRotation = new Quaternion(0, 0, 0, 0);
            playerSelect.PointersParent.SetActive(true);

            playerReady = false;
        }
    }

    private void CanSelectInput()
    {
        canSelect = true;
    }
}
