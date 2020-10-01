using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    //Almost same as ring later will be a lot different

    private AudioSource source;
    [SerializeField] private GameObject model;
    [SerializeField] private float respawnTime = 3;
    [SerializeField] private int ringCount = 10;
    [SerializeField] private float air = 15;

    private void Start()
    {
        source = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        model.SetActive(false);
        CharacterStats characterStats = other.GetComponentInParent<CharacterStats>();
        characterStats.Rings += ringCount;
        characterStats.Air += air;
        source.Play();
        Invoke("Respawn", respawnTime);
    }

    private void Respawn()
    {
        model.SetActive(true);
    }
}
