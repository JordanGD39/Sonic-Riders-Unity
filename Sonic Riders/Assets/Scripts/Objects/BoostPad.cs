using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    //private Animator canvasAnim;
    private AudioSource source;
    [SerializeField] private float boostSpeed = 0;

    private void Start()
    {
        //canvasAnim = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<Animator>();
        source = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            PlayerMovement player = other.transform.root.GetComponent<PlayerMovement>();
            CharacterStats characterStats = player.GetComponent<CharacterStats>();

            float speed = boostSpeed;

            if (boostSpeed == 0)
            {
                speed = characterStats.GetCurrentBoost() + 10;
            }

            player.Speed = speed;

            if (!player.Grounded)
            {
                other.attachedRigidbody.velocity = transform.forward * speed;
            }

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
