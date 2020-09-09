using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private float minDistance = 1;
    [SerializeField] private float maxDistance = 4;
    public float MaxDistance { set { maxDistance = value; } }
    [SerializeField] private float percentClipped = 0.87f;
    [SerializeField] private float smoothTime = 10;
    private Vector3 dollyDir;
    private float distance;
    [SerializeField] private LayerMask layerMask;

    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            return;
        }

        Vector3 desiredPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredPos, out hit, layerMask))
        {
            //Debug.Log(hit.collider.gameObject);
            distance = Mathf.Clamp((hit.distance * percentClipped), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, smoothTime * Time.deltaTime);
    }
}
