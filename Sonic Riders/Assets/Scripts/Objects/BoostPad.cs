using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            BoardStats stats = player.GetComponentInChildren<BoardStats>();

            player.Speed = stats.Limit[player.GetComponent<CharacterStats>().Level] + 10;

            player.transform.position = transform.position;
            other.transform.parent.localRotation = transform.rotation;
        }        
    }
}
