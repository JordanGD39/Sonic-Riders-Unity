using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObstacle : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    private bool invoking = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }
    
    public void Punched()
    {
        if (invoking)
        {
            return;
        }

        invoking = true;
        Invoke("ReturnToPos", 3);
    }

    private void ReturnToPos()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        invoking = false;
    }
}
