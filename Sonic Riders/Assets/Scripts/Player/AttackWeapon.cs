using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWeapon : MonoBehaviour
{
    private PlayerTrigger playerTrigger;
    private PlayerBoost playerBoost;
    private Transform model;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        playerBoost = GetComponentInParent<PlayerBoost>();
        model = GetComponentInChildren<Collider>().transform;
        playerTrigger = playerBoost.GetComponentInChildren<PlayerTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerTrigger defenderTrigger = other.GetComponent<PlayerTrigger>();
        if (other.gameObject.layer == 8 && other.isTrigger && defenderTrigger != null && other.transform != model && other.transform != playerTrigger.transform)
        {
            defenderTrigger.AttackedByPlayer(col);
        }
    }
}
