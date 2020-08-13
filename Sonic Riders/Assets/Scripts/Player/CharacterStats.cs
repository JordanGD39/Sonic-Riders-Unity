using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type { SPEED, FLIGHT, POWER, ALL}

public class CharacterStats : MonoBehaviour
{
    private HUD hud;

    [SerializeField] private int level = 0;
    public int Level { get { return level; } set { level = value; } }
    public bool IsPlayer { get; set; } = false;

    [SerializeField] private float air = 100;
    [SerializeField] private int maxAir = 100;
    public float Air { get { return air; } set { if (value > maxAir) { value = maxAir; } if (value <= 0) { RunOnFoot(); } air = value; if (IsPlayer) { hud.UpdateAirBar(air); } } }

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

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
    }

    private void RunOnFoot()
    {
        Debug.Log("Run on foot");
    }

    public bool TypeCheck(type aType)
    {
        return (aType == charType || (aType == stats.BoardType && !stats.IsStandard) || charType == type.ALL);
    }

    public float GetCurrentLimit()
    {
        return stats.Limit[level] - speedLoss;
    }

    public float GetCurrentBoost()
    {
        return stats.Boost[level];
    }
}
