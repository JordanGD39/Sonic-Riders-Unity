using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private Rigidbody rb;
    private float seaY;
    [SerializeField] private float depthBeforeSubmerged = 1;
    [SerializeField] private float displacementAmount = 3;
    [SerializeField] private float forceMultiplier = 2;
    [SerializeField] private float deeperForceMultiplier = 2;
    public float ForceMultiplier { get { return forceMultiplier; } set { forceMultiplier = value; } }
    public float DeeperForceMultiplier { get { return deeperForceMultiplier; } set { deeperForceMultiplier = value; } }
    [SerializeField] private float dragInWater = 3;
    [SerializeField] private float angularDragInWater = 1;
    [SerializeField] private float extraSeaY = 0;
    [SerializeField] private float extraDeep = 0;
    private float deepSea = 0;
    private float prevDrag = 0;
    private float prevAngularDrag = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevDrag = rb.drag;
        prevAngularDrag = rb.angularDrag;
        seaY = GameObject.FindGameObjectWithTag(Constants.Tags.sea).transform.position.y;
        deepSea = seaY - (5 + extraDeep);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < seaY + extraSeaY)
        {
            float multiplier = forceMultiplier;

            if (transform.position.y < deepSea)
            {                
                multiplier = deeperForceMultiplier;
            }

            rb.drag = dragInWater;
            rb.angularDrag = angularDragInWater;
            float displacementMultiplier = Mathf.Clamp01(((seaY - transform.position.y) / depthBeforeSubmerged) * displacementAmount);
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier * multiplier, 0), ForceMode.Acceleration);
        }
        else
        {
            rb.drag = prevDrag;
            rb.angularDrag = prevAngularDrag;
        }
    }

    private void OnJointBreak(float breakForce)
    {
        enabled = true;
    }
}
