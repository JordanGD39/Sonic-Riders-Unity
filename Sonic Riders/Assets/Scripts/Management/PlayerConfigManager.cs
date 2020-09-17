﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerConfigManager : MonoBehaviour
{
    private List<PlayerConfig> playerConfigs = new List<PlayerConfig>();

    [SerializeField] private GameObject eventSystemPref;
    private GameObject canvas;
    private EventSystemHolder eventSystemHolder;

    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag(Constants.Tags.canvas);
        eventSystemHolder = canvas.GetComponent<EventSystemHolder>();
    }

    public void SetPlayerPrefab(int index, GameObject prefab)
    {
        playerConfigs[index].CharacterPrefab = prefab;
    }

    public void PlayerReady(int index, bool ready)
    {
        playerConfigs[index].IsReady = ready;

        if (playerConfigs.All(p => p.IsReady))
        {
            GetComponent<PlayerInputManager>().DisableJoining();
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined!");

        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.parent = transform;
            playerConfigs.Add(new PlayerConfig(pi));
            GameObject eventSystem = Instantiate(eventSystemPref);
            MultiplayerEventSystem multiplayerEventSystem = eventSystem.GetComponent<MultiplayerEventSystem>();
            multiplayerEventSystem.SetSelectedGameObject(canvas.GetComponentInChildren<Button>().gameObject);
            eventSystemHolder.MultiplayerEventSystems.Add(multiplayerEventSystem);
            pi.uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        }        
    }
}

public class PlayerConfig
{
    public PlayerConfig(PlayerInput pi)
    {
        Input = pi;
        PlayerIndex = pi.playerIndex;
    }

    public PlayerInput Input { get; set; }

    public int PlayerIndex { get; set; }

    public GameObject CharacterPrefab { get; set; }

    public bool IsReady { get; set; } = false;
}
