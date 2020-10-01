using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterRelease : MonoBehaviour
{
    [SerializeField] private Transform leg;
    [SerializeField] private Transform[] legsPos;
    [SerializeField] private Transform forward;
    [SerializeField] private float launchSpeed = 66.75f;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Helicopter_Idle"))
        {
            PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();

            playerMovement.transform.parent = leg.transform;

            Rigidbody rb = playerMovement.GetComponent<Rigidbody>();

            rb.GetComponent<PlayerTricks>().CanDoTricks = false;

            //rb.GetComponent<PlayerTricks>().Landed(false);

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            playerMovement.CantMove = true;

            float distance = Mathf.Infinity;

            int nearestLeg = 0;

            for (int i = 0; i < legsPos.Length; i++)
            {
                float tempDistance = (rb.transform.position - legsPos[i].position).sqrMagnitude;

                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearestLeg = i;
                }
            }

            Transform model = transform.GetChild(0);

            Vector3 localPos = model.InverseTransformVector(rb.transform.position);
            Vector3 legLocalPos = model.InverseTransformVector(legsPos[nearestLeg].position);

            legLocalPos.z = localPos.z;
            localPos = legLocalPos;

            rb.transform.position = model.TransformVector(legLocalPos);

            rb.transform.GetChild(0).forward = legsPos[nearestLeg].forward;

            anim.Play("Helicopter_Fall");
        }        
    }

    public void ReleasePlayers()
    {
        if (leg.childCount == 0)
        {
            return;
        }

        for (int i = 0; i < leg.childCount; i++)
        {

            PlayerMovement playerMovement = leg.GetChild(i).GetComponentInParent<PlayerMovement>();
            Rigidbody rb = playerMovement.GetComponent<Rigidbody>();

            rb.transform.GetChild(0).forward = transform.forward;

            leg.GetChild(i).parent = null;

            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            playerMovement.CantMove = false;            

            rb.velocity = forward.forward * launchSpeed;

            rb.GetComponent<PlayerTricks>().CanDoTricks = true;
        }
    }
}
