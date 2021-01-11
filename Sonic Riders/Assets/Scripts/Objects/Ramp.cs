using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] private float perfectJumpRange = 7;
    private float worstPower = 7;
    [SerializeField] private float power = 30;
    [SerializeField] private float multiplier = 0.8f;
    [SerializeField] private float gravMultipler = 0.5f;
    [SerializeField] private float jumpMultiplier = 1;
    [SerializeField] private bool flightRamp = false;
    [SerializeField] private Transform differentForward;
    [SerializeField] private float jumpRotationZ = -0.5f;
    [SerializeField] private bool survivalException = false;

    public float PerfectJump { get { return perfectJumpRange; } }
    public float Power { get { return power; } }
    public float WorstPower { get { return worstPower; } }
    public float JumpMultiplier { get { return jumpMultiplier; } }
    public bool Flight { get { return flightRamp; } }
    public Transform DifferentForward { get { return differentForward; } }
    public float JumpRotationZ { get { return jumpRotationZ; } }

    private void Start()
    {
        power *= GameManager.instance.GravitityMultiplier * gravMultipler;

        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL && !survivalException)
        {
            power *= 0.8f;
            multiplier += 0.2f;

        }

        worstPower = power * multiplier;        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerTricks player = other.transform.GetComponentInParent<PlayerTricks>();

        PlayerGrind playerGrind = null;

        if (player != null)
        {
            playerGrind = player.GetComponent<PlayerGrind>();
        }
        else
        {
            return;
        }

        if (!player.CanDoTricks && !playerGrind.Grinding)
        {
            player.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerTricks player = other.transform.GetComponentInParent<PlayerTricks>();

        PlayerGrind playerGrind = null;

        if (player != null)
        {
            playerGrind = player.GetComponent<PlayerGrind>();
        }
        else
        {
            return;
        }

        if (!player.CanDoTricks && !playerGrind.Grinding)
        {
            if (flightRamp)
            {
                PlayerFlight flight = other.GetComponentInParent<PlayerFlight>();

                if (flight.enabled)
                {
                    flight.FallingOffRamp(perfectJumpRange);
                }
                else
                {
                    flight.transform.parent = null;
                }
            }
            else
            {
                other.transform.GetComponentInParent<PlayerJump>().FallingOffRamp(this);
            }
        }
    }    
}
