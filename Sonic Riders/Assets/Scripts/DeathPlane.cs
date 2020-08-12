using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerMovement mov = other.GetComponentInParent<PlayerMovement>();

            if (mov.GetComponentInChildren<CameraDeath>() != null)
            {
                mov.GetComponentInChildren<CameraDeath>().StartFollow();
            }
        }
    }
}
