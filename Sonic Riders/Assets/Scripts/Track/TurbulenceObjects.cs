using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbulenceObjects : MonoBehaviour
{
    private List<GameObject> turbulenceObjects = new List<GameObject>();
    public List<GameObject> TurbulenceList { get { return turbulenceObjects; } }

    [SerializeField] private GameObject turbulencePrefab;
    [SerializeField] private int objectsToSpawn = 80;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < objectsToSpawn; i++)
        {
            GameObject turObject = Instantiate(turbulencePrefab, transform);
            turbulenceObjects.Add(turObject);
        }
    }
}
