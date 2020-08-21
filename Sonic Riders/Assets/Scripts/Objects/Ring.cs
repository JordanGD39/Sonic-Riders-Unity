using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60;

    private AudioSource source;
    private GameObject model;

    private void Start()
    {
        model = GetComponentInChildren<MeshRenderer>().gameObject;
        source = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        model.SetActive(false);
        other.GetComponentInParent<CharacterStats>().Rings++;
        source.Play();
        Destroy(gameObject, source.clip.length);
    }
}
