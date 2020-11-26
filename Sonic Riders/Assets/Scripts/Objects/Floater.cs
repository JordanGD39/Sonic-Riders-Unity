using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private Rigidbody rb;
    private RespawnObstacle respawnObstacle;
    private float seaY;
    [SerializeField] private float depthBeforeSubmerged = 1;
    [SerializeField] private float displacementAmount = 3;
    [SerializeField] private float forceMultiplier = 2;
    public float ForceMultiplier { get { return forceMultiplier; } set { forceMultiplier = value; } }
    [SerializeField] private float dragInWater = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        respawnObstacle = GetComponent<RespawnObstacle>();
        seaY = GameObject.FindGameObjectWithTag(Constants.Tags.sea).transform.position.y;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < seaY)
        {
            rb.drag = dragInWater;
            float displacementMultiplier = Mathf.Clamp01(((seaY - transform.position.y) / depthBeforeSubmerged) * displacementAmount);
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier * forceMultiplier, 0), ForceMode.Acceleration);
        }
        else
        {
            rb.drag = 0;
        }
    }
}
