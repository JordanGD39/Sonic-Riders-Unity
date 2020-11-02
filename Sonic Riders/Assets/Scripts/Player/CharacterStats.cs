using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type { SPEED, FLIGHT, POWER, ALL}

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private string characterName = "Sonic";
    public string CharacterName { get { return characterName; } }
    [SerializeField] private Sprite portrait;
    public Sprite Portrait { get { return portrait; } }
    [SerializeField] private Sprite icon;
    public Sprite Icon { get { return icon; } }
    [SerializeField] private Color color = Color.blue;
    public Color CharColor { get { return color; } }
    [SerializeField] private float extraY = 0;
    public float ExtraY { get { return extraY; } }

    [SerializeField] private string boardName = "Blue Star";
    public string BoardName { get { return boardName; } }
    [SerializeField] private Sprite boardImage;
    public Sprite BoardImage { get { return boardImage; } }

    public int PlayerIndex { get; set; } = 4;
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
            if (survivalManager != null)
            {
                return;
            }

            switch (value)
            {
                case -1:
                    value = 0;
                    break;
                case 0:
                    maxRings = 30;
                    startRings = 0;              
                    break;
                case 1:
                    maxRings = 60;
                    startRings = 30;
                    break;
                case 2:
                    maxRings = 100;
                    startRings = 60;
                    break;
            }

            maxAir = stats.MaxAir[value];

            if (value > level || air > maxAir)
            {
                Air = maxAir;
            }

            level = value;
            if (hud != null)
                hud.UpdateLevel(level);
        }
    }
    public bool IsPlayer { get; set; } = false;
    public bool DisableAllFeatures { get; set; } = false;

    [SerializeField] private float air = 100;
    [SerializeField] private int maxAir = 100;

    [SerializeField] private int rings = 0;
    [SerializeField] private int maxRings = 30;
    [SerializeField] private int startRings = 0;

    public bool OffRoad { get; set; } = false;

    private float ringsFloat = 0;
    public float RingsFloat { get { return ringsFloat; } set { Rings = Mathf.RoundToInt(value);  ringsFloat = value; } }

    public int Rings
    {
        get { return rings; }

        set
        {
            if (survivalManager != null)
            {
                return;
            }

            if (value < 0)
            {
                value = 0;
            }
            else if (value > 100)
            {
                value = 100;
            }

            if (stats.RingsAsAir)
            {
                air = value;
            }
            else
            {
                if (value >= maxRings && level < 2)
                {
                    Level++;

                    if (value >= maxRings && level < 2)
                    {
                        Level++;
                    }
                }
                else if (value < startRings)
                {
                    Level--;

                    if (value < startRings)
                    {
                        Level--;
                    }
                }
            }            

            rings = value;

            if (hud != null)
            {
                hud.UpdateRings(rings, maxRings);
            }
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

            if (stats.RingsAsAir)
            {
                RingsFloat = value;
            }

            if (alreadyRunning && value > 0)
            {
                alreadyRunning = false;
                playerAnimation.RunningState(false);
            }

            air = value;

            if (IsPlayer && hud != null && !stats.RingsAsAir)
            {
                hud.UpdateAirBar(air, maxAir);
            }
        }
    }

    public float MaxAir { get { return maxAir; } }

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
    public BoardStats BoardStats { get { return stats; } set { stats = value; } }

    [SerializeField] private float[] boostTime = { 2, 3.5f, 4f };

    [SerializeField] private float runSpeed = 30;
    public float RunSpeed { get { return runSpeed; } }

    private bool alreadyRunning = false;

    private float timer = 0;
    public float Timer { get { return timer; } }

    public bool StopCounting { get; set; } = false;

    private SurvivalManager survivalManager;

    public bool SurvivalLeader { get; set; } = false;
    public int SurvivalScore { get; set; } = 0; 

    private void Start()
    {
        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            survivalManager = FindObjectOfType<SurvivalManager>();            
        }        
    }

    private void Update()
    {
        if (!StopCounting)
        {
            timer += Time.deltaTime;
        }        
    }

    public void GiveCanvasHud()
    {
        if (IsPlayer)
        {
            hud = Canvas.GetComponent<HUD>();
            hud.LevelThreeMaxAir = stats.MaxAir[2];           

            if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
            {
                level = 2;
                rings = 100;
                maxRings = 100;
                maxAir = 200;
                air = 0;

                if (hud != null)
                {
                    hud.UpdateRings(rings, maxRings);
                    hud.UpdateLevel(level);
                    hud.UpdateAirBar(air, maxAir);
                }
            }

            if (hud != null && stats.RingsAsAir)
            {
                hud.UpdateAirBar(0, 0);
            }
        }
    }

    private void RunOnFoot()
    {
        if (playerAnimation == null)
        {
            playerAnimation = GetComponent<PlayerAnimationHandler>();
        }

        alreadyRunning = true;
        playerAnimation.RunningState(true);
        Debug.Log("Run on foot");

        if (survivalManager != null)
        {
            survivalManager.MoveChaosEmerald(gameObject);
        }
    }

    public bool TypeCheck(type aType)
    {
        return aType == charType || (aType == stats.BoardType && stats.ChangeType) || charType == type.ALL;
    }

    public float GetCurrentLimit()
    {
        float speed = 0;

        float multiplier = 1;

        if (survivalManager)
        {
            multiplier = SurvivalLeader ? 0.8f : 1.2f;
        }

        float limit = stats.Limit + (4.3f * level) - speedLoss;

        if (OffRoad)
        {
            speed = stats.Power[level] + extraPower;

            return speed * multiplier;
        }

        if (air <= 0)
        {
            speed = runSpeed;
        }
        else
        {
            speed = limit;
        }

        return speed * multiplier;
    }

    public float GetCurrentBoost()
    {
        float multiplier = 1;

        if (survivalManager)
        {
            if (SurvivalLeader)
            {
                multiplier = 0.8f;
            }
        }

        return stats.Boost[level] * multiplier;
    }

    public float GetCurrentBoostDepletion()
    {
        float multiplier = 1;

        if (survivalManager)
        {
            if (!SurvivalLeader)
            {
                multiplier = 0.5f;
            }
        }

        return stats.BoostDepletion[level] * multiplier;
    }

    public float GetCurrentDriftDepletion()
    {
        float multiplier = 1;

        if (survivalManager)
        {
            if (!SurvivalLeader)
            {
                multiplier = 0.5f;
            }
        }

        return stats.DriftDepletion[level] * multiplier;
    }

    public float GetCurrentBoostTime()
    {
        return boostTime[level];
    }

    public float GetCurrentPower()
    {
        return stats.Power[level] + extraPower;
    }

    public float GetCurrentDash()
    {
        if (air <= 0)
        {
            return 13;
        }

        return stats.Dash + extraDash;
    }

    public float GetCurrentCornering()
    {
        if (air <= 0)
        {
            return 80;
        }

        return stats.Cornering + extraCornering;
    }

    public float GetCurrentAirLoss()
    {
        float multiplier = 1;

        if (survivalManager)
        {
            if (SurvivalLeader)
            {
                multiplier = 8;
            }
        }

        float airLoss = stats.AirDepletion - lessAirLoss;

        if (BoardStats.RingsAsAir)
        {
            airLoss += lessAirLoss;
        }

        if (airLoss < 0)
        {
            airLoss = 0;
        }

        return airLoss * multiplier;
    }

    public float GetCurrentTornadoCost()
    {
        return stats.TornadoCost[level];
    }
}
