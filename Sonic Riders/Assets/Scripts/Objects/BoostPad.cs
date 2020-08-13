using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private Animator canvasAnim;
    private AudioSource source;

    private void Start()
    {
        canvasAnim = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animator>();
        source = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            PlayerControls controls = player.GetComponent<PlayerControls>();
            CharacterStats characterStats = player.GetComponent<CharacterStats>();

            player.Speed = characterStats.BoardStats.Limit[characterStats.Level] + 10;

            player.transform.position = transform.position;
            other.transform.parent.forward = transform.forward;

            if (!source.isPlaying)
            {
                source.Play();
            }

            if (controls != null)
            {
                canvasAnim.Play("BoostCircle");
            }
        }        
    }
}
