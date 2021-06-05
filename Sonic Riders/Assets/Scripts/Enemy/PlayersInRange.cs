using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInRange : MonoBehaviour
{
    private List<GameObject> playersInRange = new List<GameObject>();
    public List<GameObject> PlayersInRangeList { get { return playersInRange; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            playersInRange.Add(other.transform.root.gameObject);
            return;
        }

        if (other.transform.parent.parent.gameObject.CompareTag("Player"))
        {
            playersInRange.Add(other.transform.parent.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform.root.gameObject);
            return;
        }

        if (other.transform.parent.parent.gameObject.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform.parent.parent.gameObject);
        }
    }
}
