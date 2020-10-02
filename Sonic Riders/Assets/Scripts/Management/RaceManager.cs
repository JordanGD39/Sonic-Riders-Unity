using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private List<CharacterStats> playersStats = new List<CharacterStats>();

    [SerializeField] private List<PlayerCheckpoints> playerCheckpoints = new List<PlayerCheckpoints>();
    public List<PlayerCheckpoints> PlayerPlacing { get { return playerCheckpoints; } }

    [SerializeField] private int laps = 3;
    public int Laps { get { return laps; } }

    [SerializeField] private Sprite[] numberSprites;
    public Sprite[] NumberSprites { get { return numberSprites; } }

    // Start is called before the first frame update
    public void AddPlayers()
    {
        List<GameObject> playerObjects = new List<GameObject>();

        playerObjects.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

        for (int i = 0; i < playerObjects.Count; i++)
        {
            playersStats.Add(playerObjects[i].GetComponent<CharacterStats>());
            playerCheckpoints.Add(playerObjects[i].GetComponent<PlayerCheckpoints>());
        }

        //InvokeRepeating("ManualUpdate", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        playerCheckpoints.Sort(CompareRider);
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
