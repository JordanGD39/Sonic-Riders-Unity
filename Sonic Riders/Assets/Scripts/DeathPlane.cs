using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CharacterStats stats = other.GetComponentInParent<CharacterStats>();

            if (stats.GetComponentInChildren<CameraDeath>() != null && stats.IsPlayer)
            {
                stats.GetComponentInChildren<CameraDeath>().StartFollow();
            }
            else
            {
                stats.transform.position = new Vector3(0, 0.4f, 0);
            }
        }
    }
}
