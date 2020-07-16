using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostOnAnimation : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerBoost playerBoost;
    private PlayerMovement playerMovement;
    //private CameraShake cameraShake;
    

    private void Start()
    {
        playerBoost = GetComponentInParent<PlayerBoost>();
        playerMovement = GetComponentInParent<PlayerMovement>();

        //cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void BoostNow()
    {
        if (!playerMovement.Grounded)
        {
            playerBoost.Boosting = false;
            return;
        }

        playerBoost.Boost();
        //StartCoroutine(cameraShake.Shake(0.1f, 0.0001f));     
    }
}
