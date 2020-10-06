using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoints : MonoBehaviour
{
    private RaceManager raceManager;
    public RaceManager RaceManagerScript { get { return raceManager; } }
    public CharacterStats CharStats { get; set; }
    private HUD hud;

    [SerializeField] private int currCheckpoint = 0;
    public int CurrCheckpoint { get { return currCheckpoint; } set { currCheckpoint = value; } }

    [SerializeField] private int lapCount = 0;
    public int LapCount { get { return lapCount; } set { lapCount = value; } }

    private int place = 0;

    private int nextCheckpointIndex = 1;

    private void Start()
    {
        raceManager = GameObject.FindGameObjectWithTag(Constants.Tags.raceManager).GetComponent<RaceManager>();
        CharStats = GetComponent<CharacterStats>();
    }

    public void GiveHud(HUD aHud)
    {
        hud = aHud;
        hud.GiveRaceManager(raceManager);
        hud.UpdateLap(lapCount, raceManager.Laps);
    }

    private void Update()
    {
        place = raceManager.PlayerPlacing.IndexOf(this);

        if (hud != null && hud.Place != place)
        {
            hud.Place = place;
            hud.ChangePlacing();
        }
    }

    public Vector3 GetNextCheckpoinDir()
    {
        Transform nextCheckpoint = raceManager.transform.GetChild(nextCheckpointIndex);

        return nextCheckpoint.forward;
    }

    private void CalcNextCheckpoint()
    {
        nextCheckpointIndex = currCheckpoint + 1;

        if (nextCheckpointIndex >= raceManager.transform.childCount)
        {
            nextCheckpointIndex = 0;
        }
    }

    public void CheckCheckpoint(Transform checkpoint)
    {
        int checkpointIndex = checkpoint.GetSiblingIndex();

        bool lastCheckpointReached = checkpointIndex == 0 && currCheckpoint == raceManager.transform.childCount - 1;

        if (checkpointIndex == currCheckpoint + 1 || lastCheckpointReached)
        {
            currCheckpoint = checkpointIndex;
            CalcNextCheckpoint();

            if (lastCheckpointReached)
            {
                lapCount++;
                hud.UpdateLap(lapCount, raceManager.Laps);

                if (lapCount >= raceManager.Laps)
                {
                    currCheckpoint = 100 - place;
                    raceManager.CheckRaceEnd(this);
                }
            }
        }
    }
}
