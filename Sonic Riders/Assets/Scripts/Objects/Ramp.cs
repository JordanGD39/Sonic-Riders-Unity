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
        worstPower = power * multiplier;
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
                other.transform.GetComponentInParent<PlayerJump>().FallingOffRamp(this);
            }
        }
    }    
}
