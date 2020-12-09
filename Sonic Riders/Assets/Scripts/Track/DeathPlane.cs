using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        KillPlayer(other.gameObject);
    }

    private void KillPlayer(GameObject other)
    {
        if (other.layer == 8)
        {
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();

            if (rb.GetComponentInChildren<CameraDeath>() != null)
            {
                rb.GetComponentInChildren<CameraDeath>().StartFollow();
            }
        }
    }
}
