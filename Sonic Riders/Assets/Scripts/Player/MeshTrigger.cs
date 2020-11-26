using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrigger : MonoBehaviour
{
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer == 14)
        {
            playerMovement.OnTrack = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == 14)
        {
            playerMovement.OnTrack = false;
        }
    }
}

