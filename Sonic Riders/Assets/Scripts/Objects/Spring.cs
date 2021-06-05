using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private float launchSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CharacterStats characterStats = other.transform.root.GetComponent<CharacterStats>();
            Transform player = characterStats.transform;
            //player.transform.GetChild(0).up = transform.up;
            other.attachedRigidbody.velocity = transform.up * launchSpeed;

            if (characterStats.IsPlayer)
            {
                audioSource.Play();
            }
        }
    }
}
