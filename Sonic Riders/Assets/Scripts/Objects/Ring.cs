using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 60;
    [SerializeField] private float respawnTime = 3;

    private AudioSource source;
    private GameObject model;
    private Renderer ringRenderer;

    private LODGroup lod;

    private void Start()
    {
        ringRenderer = GetComponentInChildren<MeshRenderer>();
        model = ringRenderer.gameObject;
        source = GetComponentInChildren<AudioSource>();
        lod = GetComponent<LODGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ringRenderer.isVisible)
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 8)
        {
            return;
        }

        model.SetActive(false);
        other.GetComponentInParent<CharacterStats>().Rings++;
        source.Play();
        Invoke("Respawn", respawnTime);
    }

    private void Respawn()
    {
        model.SetActive(true);
    }
}
