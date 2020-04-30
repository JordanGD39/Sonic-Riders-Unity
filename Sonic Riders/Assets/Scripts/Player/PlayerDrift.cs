using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrift : MonoBehaviour
{
    private PlayerMovement movement;
    private BoardStats stats;
    private CharacterStats charStats;
    public bool DriftPressed { get; set; } = false;
    public float DriftDir { get; set; } = 0;

    private bool driftActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        stats = transform.GetChild(0).GetChild(1).GetComponent<BoardStats>();
        charStats = GetComponent<CharacterStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.Grounded /*&& charStats.Air > 0*/)
        {
            if (DriftPressed)
            {
                if (movement.TurnAmount != 0 || driftActivated)
                {
                    Drift();
                }
                else
                {
                    //Braking
                    if (movement.Speed > 0)
                    {
                        movement.Speed -= 30 * Time.deltaTime;
                    }
                    else
                    {
                        movement.Speed = 0;
                    }                    
                }
                
                return;
            }
        }

        movement.Drifting = false;
        driftActivated = false;
        DriftDir = 0;
    }

    private void Drift()
    {
        if (DriftDir == 0)
        {
            if (movement.TurnAmount > 0)
            {
                DriftDir = 1;
            }
            else
            {
                DriftDir = -1;
            }
        }

        if (movement.Speed <= 0)
        {
            movement.Drifting = false;
            driftActivated = false;
            return;
        }

        driftActivated = true;
        movement.Drifting = true;
        movement.Speed -= 4 * Time.deltaTime;
        charStats.Air -= stats.AirDepletion * 1.5f;
    }
}
