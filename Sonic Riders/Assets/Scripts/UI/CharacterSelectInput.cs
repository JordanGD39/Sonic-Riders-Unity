using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CharacterSelectInput : MonoBehaviour
{
    private bool canSelect = false;
    public bool CanSelect { get { return canSelect; } }

    private PlayerInput playerInput;
    private PlayerConfigManager playerConfigManager;
    private MultiplayerEventSystem eventSystem;
    private GameObject button;

    // Start is called before the first frame update
    void Start()
    {
        button = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponentInChildren<Button>().gameObject;
        playerConfigManager = GameManager.instance.GetComponent<PlayerConfigManager>();

        InputMaster inputMaster = new InputMaster();
        playerInput = GetComponent<PlayerInput>();
        eventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
        playerInput.actions.FindAction(inputMaster.UI.Cancel.id).performed += ctx => CancelSelection();

        Invoke("CanSelectInput", 1);
    }

    public void ConfirmCharacter(GameObject prefab)
    {
        playerConfigManager.SetPlayerPrefab(playerInput.playerIndex, prefab);
        playerConfigManager.PlayerReady(playerInput.playerIndex, true);
    }

    private void CancelSelection()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(button);
            playerConfigManager.PlayerReady(playerInput.playerIndex, false);
        }
    }

    private void CanSelectInput()
    {
        canSelect = true;
    }
}
