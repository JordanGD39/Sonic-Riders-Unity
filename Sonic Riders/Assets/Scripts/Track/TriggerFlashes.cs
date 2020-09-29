using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFlashes : MonoBehaviour
{
    [SerializeField] private GameObject flashes;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            flashes.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            flashes.SetActive(false);
        }
    }
}
