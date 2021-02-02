using System.Collections;
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
    [SerializeField] private bool dontFindCanvas = true;
    private PlayerInputManager playerInputManager;
    public int MaxPlayers { get; set; } = 4;

    private bool canJoin = false;

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        if (!dontFindCanvas)
        {
            FindCanvas();
        }
        else
        {
            playerInputManager.DisableJoining();
        }
    }

    public void SetPlayerPrefab(int index, GameObject prefab, BoardStats boardStats)
    {
        if (index >= playerConfigs.Count || index < 0)
        {
            Debug.LogError(index + " is not in player configs!");
            return;
        }

        playerConfigs[index].CharacterPrefab = prefab;
        playerConfigs[index].GearStats = boardStats;
    }

    private void UnreadyPlayers()
    {
        for (int i = 0; i < playerConfigs.Count; i++)
        {
            playerConfigs[i].IsReady = false;
        }
    }

    public void PlayerReady(int index, bool ready)
    {
        playerConfigs[index].IsReady = ready;

        if (playerConfigs.All(p => p.IsReady))
        {
            playerInputManager.DisableJoining();
            GameManager.instance.LoadScene(GameManager.instance.TrackToLoad, true);
        }
    }

    public void RemovePlayer(int index)
    {
        playerInputManager.DisableJoining();
        playerConfigs.RemoveAt(index);

        Invoke("CanJoinAgain", 0.25f);
    }

    private void CanJoinAgain()
    {
        playerInputManager.EnableJoining();
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player " + pi.playerIndex + " joined!");

        if (playerInputManager.playerCount >= MaxPlayers)
        {
            playerInputManager.DisableJoining();
        }

        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.parent = transform.GetChild(0);
            playerConfigs.Add(new PlayerConfig(pi));
            CreateEventSystem(pi);
        }        
    }

    public void FindCanvas()
    {
        canvas = GameObject.FindGameObjectWithTag(Constants.Tags.canvas);
        eventSystemHolder = canvas.GetComponent<EventSystemHolder>();
    }

    public void CreateEventSystem(PlayerInput pi)
    {
        GameObject eventSystem = Instantiate(eventSystemPref);
        MultiplayerEventSystem multiplayerEventSystem = eventSystem.GetComponent<MultiplayerEventSystem>();
        eventSystemHolder.MultiplayerEventSystems.Add(multiplayerEventSystem);
        pi.uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
    }

    public void ClearEventSystems()
    {
        UnreadyPlayers();
        eventSystemHolder.MultiplayerEventSystems.Clear();
    }

    public void SpawnPlayers(StartingLevel startingLevel)
    {
        GameManager.instance.PlayersLeft.Clear();
        GameManager.instance.PlayersNames.Clear();

        for (int i = 0; i < playerConfigs.Count; i++)
        {
            GameObject character = Instantiate(playerConfigs[i].CharacterPrefab, new Vector3(0, 0.4f, 0), Quaternion.identity);
            character.GetComponent<CharacterStats>().BoardStats = playerConfigs[i].GearStats;
            GameManager.instance.PlayersLeft.Add(character);
        }

        GameManager.instance.GetCams();
        startingLevel.PlacePlayersInOrder();
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

    public BoardStats GearStats { get; set; }

    public bool IsReady { get; set; } = false;
}
