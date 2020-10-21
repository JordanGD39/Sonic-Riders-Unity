using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestHandleJoin : MonoBehaviour
{
    [SerializeField] private StartingLevel startingLevel;
    private SurvivalManager survivalManager;
    private List<GameObject> players = new List<GameObject>();
    private PlayerInput currInput;

    private void Start()
    {
        survivalManager = FindObjectOfType<SurvivalManager>();
        if (startingLevel != null)
        {
            startingLevel.PlacePlayersInOrder();
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        GetComponent<PlayerInputManager>().EnableJoining();
        pi.GetComponent<PlayerControls>().enabled = true;

        if (survivalManager == null)
        {
            return;
        }

        currInput = pi;
        StartCoroutine("WaitControls");
    }

    private IEnumerator WaitControls()
    {
        while (currInput.GetComponent<PlayerControls>().Player == null)
        {
            yield return null;
        }

        players.Add(currInput.GetComponent<PlayerControls>().Player);
        survivalManager.GetPlayers(players);
    }
}
