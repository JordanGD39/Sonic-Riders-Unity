using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailabePipeGenerators : MonoBehaviour
{
    private List<PipeMeshGenerator> pipeMeshGenerators = new List<PipeMeshGenerator>();

    public List<PipeMeshGenerator> PipeMeshGenerators { get { return pipeMeshGenerators; } }

    [SerializeField] private GameObject meshGeneratorPref;
    [SerializeField] private int spawnCount = 10;

    private void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject gen = Instantiate(meshGeneratorPref, transform);
            gen.transform.position = Vector3.zero;

            pipeMeshGenerators.Add(gen.GetComponent<PipeMeshGenerator>());
        }       
    }
}
