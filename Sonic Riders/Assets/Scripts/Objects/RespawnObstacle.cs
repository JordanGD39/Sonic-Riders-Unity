using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObstacle : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    private bool invoking = false;
    public bool Invoking { get { return invoking; } }
    [SerializeField] private float timeToRespawn = 3;
    private Floater floater;
    private float startForce = 2;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        floater = GetComponent<Floater>();

        if (floater != null)
            startForce = floater.ForceMultiplier;
    }
    
    public void Punched()
    {
        if (invoking)
        {
            return;
        }

        if (floater != null)
        {
            floater.ForceMultiplier = 1;
        }

        invoking = true;
        Invoke("ReturnToPos", timeToRespawn);
    }

    private void ReturnToPos()
    {
        floater.ForceMultiplier = startForce;
        transform.position = startPos;
        transform.rotation = startRot;
        invoking = false;
    }
}
