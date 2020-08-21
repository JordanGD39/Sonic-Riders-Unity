using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    //Almost same as ring later will be a lot different

    private AudioSource source;
    [SerializeField] private GameObject model;

    private void Start()
    {
        source = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        model.SetActive(false);
        other.GetComponentInParent<CharacterStats>().Rings += 100;
        source.Play();
        Destroy(gameObject, source.clip.length);
    }
}
