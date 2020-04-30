using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        BoardStats stats = player.transform.GetChild(0).GetComponentInChildren<BoardStats>();

        player.Speed = stats.Limit[player.GetComponent<CharacterStats>().Level] + 10;

        player.transform.position = transform.position;
        other.transform.localRotation = transform.rotation;
    }
}
