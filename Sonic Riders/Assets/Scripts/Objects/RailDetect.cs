using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class RailDetect : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    [SerializeField] private float multiplier = 1;

    private void Start()
    {
        if (path == null)
        {
            path = GetComponentInParent<PathCreator>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerGrind playerGrind = other.GetComponentInParent<PlayerGrind>();
            playerGrind.speedMultiplier = multiplier;
            playerGrind.Path = path;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && !other.GetComponentInParent<PlayerGrind>().Grinding)
        {
            other.GetComponentInParent<PlayerGrind>().Path = null;
        }
    }
}
