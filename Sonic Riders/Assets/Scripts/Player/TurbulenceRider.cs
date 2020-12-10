using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TurbulenceRider : MonoBehaviour
{
    private VertexPath vertexPath;
    public VertexPath Path { set { vertexPath = value; } }
    
    private PlayerGrind playerGrind;
    private PlayerMovement playerMovement;

    [SerializeField] private bool inTurbulence = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTurbulence)
        {
            vertexPath = null;
        }
    }

    public void CheckTurbulence()
    {
        if (!inTurbulence && !playerMovement.Grounded && vertexPath != null && vertexPath.GetClosestTimeOnPath(transform.position) < 0.99f)
        {
            inTurbulence = true;
            playerGrind.PathVertex = vertexPath;
            playerGrind.GrindPhysics = false;
            playerGrind.CheckGrind(false);
        }        
    }
}
