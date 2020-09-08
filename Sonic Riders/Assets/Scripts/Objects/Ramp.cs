using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] private float perfectJumpRange = 7;
    private float worstPower = 7;
    [SerializeField] private float power = 30;
    [SerializeField] private float multiplier = 0.8f;
    [SerializeField] private bool flightRamp = false;

    public float PerfectJump { get { return perfectJumpRange; } }
    public float Power { get { return power; } }
    public float WorstPower { get { return worstPower; } }
    public bool Flight { get { return flightRamp; } }

    private void Start()
    {
        worstPower = power * 0.8f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<PlayerMovement>().CompareTag("Player"))
        {
            other.transform.GetComponentInParent<PlayerMovement>().transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponentInParent<PlayerMovement>().CompareTag("Player"))
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
                other.transform.GetComponentInParent<PlayerJump>().FallingOffRamp(worstPower, perfectJumpRange, power);
            }
        }
    }    
}
