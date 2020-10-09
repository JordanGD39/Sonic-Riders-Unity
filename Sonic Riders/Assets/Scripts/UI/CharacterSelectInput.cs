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

    private PlayerInput playerInput;
    private PlayerConfigManager playerConfigManager;
    private MultiplayerEventSystem eventSystem;
    private GameObject prevButton;
    [SerializeField] private GameObject playerTextPref;
    private Text playerText;

    // Start is called before the first frame update

    private void Start()
    {
        StartFunctions();
    }

    public void StartFunctions()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            enabled = false;
            return;
        }

        GameObject button = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponentInChildren<Button>().gameObject;

        playerConfigManager = GameManager.instance.GetComponent<PlayerConfigManager>();

        InputMaster inputMaster = new InputMaster();
        playerInput = GetComponent<PlayerInput>();
        eventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
        playerInput.actions.FindAction(inputMaster.UI.Cancel.id).performed += ctx => CancelSelection();

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

    private void Update()
    {
        if (eventSystem != null && eventSystem.currentSelectedGameObject != null)
        {            
            playerText.transform.SetParent(eventSystem.currentSelectedGameObject.transform, false);
            playerText.transform.localPosition = new Vector3(0, 22, 0);
            prevButton = eventSystem.currentSelectedGameObject;
        }        
    }

    public void ConfirmCharacter(GameObject prefab)
    {
        playerText.gameObject.SetActive(false);
        playerConfigManager.SetPlayerPrefab(playerInput.playerIndex, prefab);
        playerConfigManager.PlayerReady(playerInput.playerIndex, true);
    }

    private void CancelSelection()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            prevButton.GetComponent<CharacterButton>().DisabledImage.SetActive(false);
            playerText.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(prevButton);
            playerConfigManager.PlayerReady(playerInput.playerIndex, false);
        }
    }

    private void CanSelectInput()
    {
        canSelect = true;
    }
}
