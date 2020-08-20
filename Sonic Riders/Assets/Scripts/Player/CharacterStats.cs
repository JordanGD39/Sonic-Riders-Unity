using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type { SPEED, FLIGHT, POWER, ALL}

public class CharacterStats : MonoBehaviour
{
    private HUD hud;
    private PlayerAnimationHandler playerAnimation;

    [SerializeField] private int level = 0;
    public int Level { get { return level; } set { level = value; } }
    public bool IsPlayer { get; set; } = false;

    [SerializeField] private float air = 100;
    [SerializeField] private int maxAir = 100;

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
    [SerializeField] private float extraCornering = 0;
    public float ExtraCornering { get { return extraCornering; } }
    [SerializeField] private type charType;
    public type CharType { get { return charType; } }
    [SerializeField] private BoardStats stats;
    public BoardStats BoardStats { get { return stats; } }

    private float runSpeed = 30;

    private bool alreadyRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<HUD>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
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
}
