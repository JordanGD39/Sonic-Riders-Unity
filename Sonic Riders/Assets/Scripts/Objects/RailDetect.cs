using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class RailDetect : MonoBehaviour
{
    private PathCreator path;

    private void Start()
    {
        path = GetComponentInParent<PathCreator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerGrind playerGrind = other.GetComponentInParent<PlayerGrind>();
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
