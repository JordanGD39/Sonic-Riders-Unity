using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BuoySpawn : MonoBehaviour
{
    [SerializeField] private int spawnCount = 100;
    [SerializeField] private GameObject buoyPref;

    // Start is called before the first frame update
    void Start()
    {
        PathCreator path = GetComponent<PathCreator>();
        float timePerBuoy = 1 / (float)spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            if (i > 0)
            {
                Vector3 spawnPos = path.path.GetPointAtTime(timePerBuoy * i, EndOfPathInstruction.Stop);

                Debug.Log(spawnPos);

                GameObject buoy = Instantiate(buoyPref, spawnPos, Quaternion.identity);
                buoy.transform.forward = path.path.GetDirection(timePerBuoy * i, EndOfPathInstruction.Stop);
                buoy.transform.SetParent(transform, true);
            }            
        }
    }
}
