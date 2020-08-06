using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] private float perfectJumpRange = 7;
    [SerializeField] private float worstPower = 7;
    [SerializeField] private float power = 30;
    [SerializeField] private float speed = 20;

    public float PerfectJump { get { return perfectJumpRange; } }
    public float Power { get { return power; } }
    public float Speed { get { return speed; } }

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
            other.transform.GetComponentInParent<PlayerJump>().FallingOffRamp(worstPower, speed, perfectJumpRange);
        }
    }    
}
