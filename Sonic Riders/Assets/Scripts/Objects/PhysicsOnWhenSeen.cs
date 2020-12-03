using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsOnWhenSeen : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnBecameVisible()
    {
        rb.isKinematic = false;
    }

    private void OnBecameInvisible()
    {
        rb.isKinematic = true;
    }
}
