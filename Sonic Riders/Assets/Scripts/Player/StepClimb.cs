using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepClimb : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    [SerializeField] private Transform stepRayUpper;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] private float stepSmooth = 2f;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();        
        playerMovement = GetComponent<PlayerMovement>();        
    }

    private void FixedUpdate()
    {
        if (playerMovement.Grounded)
        {
            Climb();
        }
    }

    private void Climb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.position, transform.GetChild(0).forward, out hitLower, 0.1f, layerMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.position, transform.GetChild(0).forward, out hitUpper, 0.2f, layerMask))
            {
                rb.position += transform.GetChild(0).up * stepSmooth * Time.deltaTime;
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.position, transform.GetChild(0).TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f, layerMask))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.position, transform.GetChild(0).TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f, layerMask))
            {
                rb.position += transform.GetChild(0).up * stepSmooth * Time.deltaTime;
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.position, transform.GetChild(0).TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f, layerMask))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.position, transform.GetChild(0).TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f, layerMask))
            {
                rb.position += transform.GetChild(0).up * stepSmooth * Time.deltaTime;
            }
        }
    }
}