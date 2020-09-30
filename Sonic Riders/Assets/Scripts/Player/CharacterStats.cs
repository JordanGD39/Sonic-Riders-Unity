﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type { SPEED, FLIGHT, POWER, ALL}

public class CharacterStats : MonoBehaviour
{
    public int PlayerIndex { get; set; } = 0;
    public Transform Cam { get; set; }
    public Transform Canvas { get; set; }
    public Vector3 CamStartPos { get; set; }
    private HUD hud;
    private PlayerAnimationHandler playerAnimation;

    [SerializeField] private int level = 0;
    public int Level
    {
        get { return level; }
        set
        {
            switch (value)
            {
                case -1:
                    value = 0;
                    break;
                case 0:
                    maxRings = 30;
                    maxAir = 100;                    
                    break;
                case 1:
                    maxRings = 60;
                    maxAir = 200;
                    break;
                case 2:
                    maxRings = 100;
                    maxAir = 300;
                    break;
            }

            if (value > level || air > maxAir)
            {
                Air = maxAir;
            }

            level = value;
            hud.UpdateLevel(level);
        }
    }
    public bool IsPlayer { get; set; } = false;

    [SerializeField] private float air = 100;
    [SerializeField] private int maxAir = 100;

    [SerializeField] private int rings = 0;
    [SerializeField] private int maxRings = 30;

    public bool OffRoad { get; set; } = false;

    public int Rings
    {
        get { return rings; }

        set
        {
            if (value >= maxRings)
            {
                if (maxRings > 60)
                {
                    value = maxRings;
                }
                else
                {
                    value -= maxRings;

                    Level++;

                    if (value >= maxRings)
                    {
                        value -= maxRings;

                        Level++;
                    }
                }                
            }

            rings = value;
            hud.UpdateRings(rings, maxRings);
        }
    }

    public float Air
    {
        get { return air; }

        set
        {
            if (value > maxAir)
            {
                value = maxAir;
            }

            if (value <= 0)
            {
                if (!alreadyRunning)
                {
                    RunOnFoot();
                }

                value = 0;
            }

            if (alreadyRunning && value > 0)
            {
                alreadyRunning = false;
                playerAnimation.RunningState(false);
            }

            air = value;

            if (IsPlayer)
            {
                hud.UpdateAirBar(air);
            }
        }
    }

    [SerializeField] private float speedLoss = 0;
    public float SpeedLoss { get { return speedLoss; } }
    [SerializeField] private float extraPower = 0;
    public float ExtraPower { get { return extraPower; } }
    [SerializeField] private float extraDash = 0;
    public float ExtraDash { get { return extraDash; } }
    [SerializeField] private float extraCornering = 0;
    public float ExtraCornering { get { return extraCornering; } }
    [SerializeField] private float lessAirLoss = 0;
    public float LessAirLoss { get { return lessAirLoss; } }
    [SerializeField] private type charType;
    public type CharType { get { return charType; } }
    [SerializeField] private BoardStats stats;
    public BoardStats BoardStats { get { return stats; } }

    [SerializeField] private float runSpeed = 30;

    private bool alreadyRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        //hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<HUD>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
    }

    public void GiveCanvasHud()
    {
        hud = Canvas.GetComponent<HUD>();
    }

    private void RunOnFoot()
    {
        alreadyRunning = true;
        playerAnimation.RunningState(true);
        Debug.Log("Run on foot");
    }

    public bool TypeCheck(type aType)
    {
        return (aType == charType || (aType == stats.BoardType && !stats.IsStandard) || charType == type.ALL);
    }

    public float GetCurrentLimit()
    {
        float speed = 0;

        if (OffRoad)
        {
            return speed = stats.Power[level] + extraPower;
        }

        if (air <= 0)
        {
            speed = runSpeed;
        }
        else
        {
            speed = stats.Limit[level] - speedLoss;
        }

        return speed;
    }

    public float GetCurrentBoost()
    {
        return stats.Boost[level];
    }

    public float GetCurrentPower()
    {
        return stats.Power[level] + extraPower;
    }

    public float GetCurrentDash()
    {
        return stats.Dash + extraDash;
    }

    public float GetCurrentCornering()
    {
        return stats.Cornering + extraCornering;
    }

    public float GetCurrentAirLoss()
    {
        return stats.AirDepletion - lessAirLoss;
    }
}
