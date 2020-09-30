using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestHandleJoin : MonoBehaviour
{
    [SerializeField] private StartingLevel startingLevel;

    private void Start()
    {
        startingLevel.PlacePlayersInOrder();
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        pi.GetComponent<PlayerControls>().enabled = true;
    }
}
