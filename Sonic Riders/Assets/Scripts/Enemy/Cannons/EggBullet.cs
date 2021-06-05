using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBullet : MonoBehaviour
{
    public delegate void BulletCollisionDelegate();
    public BulletCollisionDelegate bulletCollision;
    private Rigidbody rb;
    public bool targetReady = false;
    [SerializeField] private float outOfBoundsY = -100;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private LayerMask explosionLayerMask;
    private GameObject explosionParticles;
    private Vector3 savedPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        explosionParticles = Instantiate(explosionPrefab, Vector3.zero, Quaternion.identity);
        explosionParticles.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (transform.position.y < outOfBoundsY)
        {
            bulletCollision();
        }

        if (transform.localPosition != Vector3.zero)
        {
            savedPos = transform.position;
        }

        if (targetReady)
        {
            // update the rotation of the projectile during trajectory motion
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 0 && other.gameObject.layer != 8 && other.gameObject.layer != 13)
        {
            return;
        }

        bool foundGroundPlayerHit = false;
        Debug.Log(other.gameObject);

        if (other.gameObject.layer == 8)
        {
            RaycastHit hit;
            Debug.Log("Check");
            if (Physics.Raycast(savedPos, -Vector3.up, out hit, 100, explosionLayerMask))
            {
                Debug.Log(hit.collider.gameObject);
                explosionParticles.transform.position = hit.point + Vector3.up * 0.01f;
                foundGroundPlayerHit = true;
            }
        }
        
        if(!foundGroundPlayerHit)
        {
            explosionParticles.transform.position = savedPos;
        }

        explosionParticles.SetActive(false);
        explosionParticles.SetActive(true);
        bulletCollision();
    }
}
