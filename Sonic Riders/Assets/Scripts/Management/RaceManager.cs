using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private List<PlayerCheckpoints> playerCheckpoints = new List<PlayerCheckpoints>();
    private List<PlayerCheckpoints> playersLeft = new List<PlayerCheckpoints>();
    public List<PlayerCheckpoints> PlayerPlacing { get { return playerCheckpoints; } }

    [SerializeField] private int laps = 3;
    public int Laps { get { return laps; } }

    [SerializeField] private Sprite[] numberSprites;
    private BigCanvasUI bigCanvasUI;

    public Sprite[] NumberSprites { get { return numberSprites; } }

    private bool playingVictory = false;

    private void Start()
    {
        playingVictory = false;
        bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();
    }

    public void AddPlayers()
    {
        List<GameObject> playerObjects = new List<GameObject>();

        playerObjects.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

        for (int i = 0; i < playerObjects.Count; i++)
        {
            PlayerCheckpoints player = playerObjects[i].GetComponent<PlayerCheckpoints>();
            playerCheckpoints.Add(player);

            if (playerObjects[i].GetComponent<CharacterStats>().IsPlayer)
            {
                playersLeft.Add(player);
            }
        }

        //InvokeRepeating("ManualUpdate", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        playerCheckpoints.Sort(CompareRider);
    }

    public void CheckRaceEnd(PlayerCheckpoints player)
    {
        if (!playingVictory)
        {
            playingVictory = true;
            GameManager.instance.GetAudioManager.StopPlaying(GameManager.instance.GetAudioManager.CurrSound.name);
            GameManager.instance.GetAudioManager.Play("Victory");
        }

        playersLeft.Remove(player);

        if (playersLeft.Count == 0)
        {
            bigCanvasUI.PostPlacings(playerCheckpoints);
        }
    }

    private int CompareRider(PlayerCheckpoints a, PlayerCheckpoints b)
    {
        //Checking vehicle laps
        if (a.LapCount < b.LapCount)
        {
            return 1;
        }

        if (a.LapCount > b.LapCount)
        {
            return -1;
        }

        //Vehicles are on the same lap so we check if one vehicle is ahead in checkpoints
        if (a.CurrCheckpoint < b.CurrCheckpoint)
        {
            return 1;
        }

        if (a.CurrCheckpoint > b.CurrCheckpoint)
        {
            return -1;
        }

        //They're equal in checkpoint count so we must do a distance check
        if (a.CurrCheckpoint == b.CurrCheckpoint)
        {
            Vector3 nextCheckpointDir = a.GetNextCheckpoinDir();
            Vector3 diffDir = a.transform.position - b.transform.position;

            float dot = Vector3.Dot(nextCheckpointDir, diffDir);

            if (dot < 0)
            {
                return 1;
            }

            if (dot > 0)
            {
                return -1;
            }
        }

        return 0;
    }
}
