using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    //private Animator canvasAnim;
    private AudioSource source;

    private void Start()
    {
        //canvasAnim = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<Animator>();
        source = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            CharacterStats characterStats = player.GetComponent<CharacterStats>();

            player.Speed = characterStats.GetCurrentBoost() + 10;

            player.transform.position = transform.position;
            other.transform.parent.forward = transform.forward;
            other.transform.parent.localRotation = new Quaternion(0, other.transform.parent.localRotation.y, 0, other.transform.parent.localRotation.w);

            if (!source.isPlaying)
            {
                source.Play();
            }

            if (characterStats.IsPlayer)
            {
                characterStats.Canvas.GetComponent<Animator>().Play("BoostCircle");
            }
        }        
    }
}
