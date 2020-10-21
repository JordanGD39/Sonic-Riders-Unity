﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoints : MonoBehaviour
{
    private RaceManager raceManager;
    public RaceManager RaceManagerScript { get { return raceManager; } }
    public CharacterStats CharStats { get; set; }
    private PlayerMovement playerMovement;
    private AudioManagerHolder audioHolder;
    private HUD hud;
    private SurvivalManager survivalManager;

    [SerializeField] private int currCheckpoint = 0;
    public int CurrCheckpoint { get { return currCheckpoint; } set { currCheckpoint = value; } }

    [SerializeField] private int lapCount = 1;
    public int LapCount { get { return lapCount; } set { lapCount = value; } }

    private int place = 0;

    private int nextCheckpointIndex = 1;

    public void GiveHud(HUD aHud)
    {
        raceManager = GameObject.FindGameObjectWithTag(Constants.Tags.raceManager).GetComponent<RaceManager>();
        CharStats = GetComponent<CharacterStats>();
        playerMovement = GetComponent<PlayerMovement>();
        audioHolder = GetComponent<AudioManagerHolder>();

        if (GameManager.instance.GameMode != GameManager.gamemode.RACE)
        {
            aHud.PlacingUI.SetActive(false);
            survivalManager = FindObjectOfType<SurvivalManager>();
            enabled = false;
            return;
        }
        else
        {
            aHud.PlacingUI.SetActive(true);
        }

        if (aHud == null)
        {
            return;
        }

        hud = aHud;
        hud.GiveRaceManager(raceManager);
        hud.UpdateLap(lapCount, raceManager.Laps);
    }

    private void Update()
    {
        if (raceManager == null)
        {
            return;
        }

        place = raceManager.PlayerPlacing.IndexOf(this);

        if (hud != null && hud.Place != place)
        {
            hud.Place = place;
            hud.ChangePlacing();
        }
    }

    public Vector3 GetNextCheckpoinDir()
    {
        if (raceManager == null)
        {
            return Vector3.zero;
        }

        Transform nextCheckpoint = raceManager.transform.GetChild(nextCheckpointIndex);

        return nextCheckpoint.forward;
    }

    public int CalcNextCheckpoint(int skipIndex)
    {
        int nextIndex = currCheckpoint + skipIndex;

        if (nextIndex >= raceManager.transform.childCount)
        {
            nextIndex -= raceManager.transform.childCount;
        }

        return nextIndex;
    }

    public void CheckCheckpoint(Transform checkpoint)
    {
        int checkpointIndex = checkpoint.GetSiblingIndex();

        bool lastCheckpointReached = checkpointIndex == 0 && currCheckpoint == raceManager.transform.childCount - 1;

        if (checkpointIndex == currCheckpoint + 1 || lastCheckpointReached)
        {
            currCheckpoint = checkpointIndex;
            nextCheckpointIndex = CalcNextCheckpoint(1);

            if (lastCheckpointReached)
            {
                if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
                {
                    survivalManager.CheckLeaderLap(gameObject);
                    return;
                }

                lapCount++;

                if (lapCount > raceManager.Laps)
                {
                    if (CharStats.IsPlayer)
                    {
                        if (place == 0)
                        {
                            audioHolder.VoiceManager.Play(Constants.VoiceSounds.win);
                        }
                        else
                        {
                            audioHolder.VoiceManager.Play(Constants.VoiceSounds.lose);
                        }
                    }                    

                    currCheckpoint = 100 - place;
                    CharStats.DisableAllFeatures = true;
                    CharStats.StopCounting = true;
                    raceManager.CheckRaceEnd(this);                    
                }

                if (hud != null)
                {
                    hud.UpdateLap(lapCount, raceManager.Laps);
                }
            }
        }
    }
}
