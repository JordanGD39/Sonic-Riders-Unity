using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private HUD hud;

    [SerializeField] private int level = 0;
    public int Level { get { return level; } set { level = value; } }

    [SerializeField] private float air = 100;
    [SerializeField] private int maxAir = 100;
    public float Air { get { return air; } set { if (value > maxAir) { value = maxAir; } if (value <= 0) { RunOnFoot(); } air = value; hud.UpdateAirBar(air); } }    

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
    }

    private void RunOnFoot()
    {
        Debug.Log("Run on foot");
    }
}
