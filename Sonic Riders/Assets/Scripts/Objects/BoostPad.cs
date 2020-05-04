using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.transform.parent.GetComponentInParent<PlayerMovement>();
        BoardStats stats = player.GetComponentInChildren<BoardStats>();

        player.Speed = stats.Limit[player.GetComponent<CharacterStats>().Level] + 10;

        player.transform.position = transform.position;
        other.transform.parent.localRotation = transform.rotation;
    }
}
