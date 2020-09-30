using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringSound : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CharacterStats characterStats = other.GetComponentInParent<CharacterStats>();

            if (characterStats.IsPlayer)
            {
                audioSource.Play();
            }
        }
    }
}
