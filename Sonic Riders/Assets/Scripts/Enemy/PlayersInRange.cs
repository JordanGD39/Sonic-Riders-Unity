using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInRange : MonoBehaviour
{
    private List<GameObject> playersInRange = new List<GameObject>();
    private List<Rigidbody> rigidBodiesInRange = new List<Rigidbody>();
    public List<Rigidbody> RigidBodiesInRange { get { return rigidBodiesInRange; } }
    public List<GameObject> PlayersInRangeList { get { return playersInRange; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            playersInRange.Add(other.transform.root.gameObject);
            rigidBodiesInRange.Add(other.attachedRigidbody);
            return;
        }

        if (other.transform.parent.parent.gameObject.CompareTag("Player"))
        {
            playersInRange.Add(other.transform.parent.parent.gameObject);
            rigidBodiesInRange.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform.root.gameObject);
            rigidBodiesInRange.Remove(other.attachedRigidbody);
            return;
        }

        if (other.transform.parent.parent.gameObject.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform.parent.parent.gameObject);
            rigidBodiesInRange.Remove(other.attachedRigidbody);
        }
    }
}
