using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBullet : MonoBehaviour
{
    public delegate void BulletCollisionDelegate(bool hitPlayer);
    public BulletCollisionDelegate bulletCollision;
    private Rigidbody rb;
    public bool targetReady = false;
    [SerializeField] private float outOfBoundsY = -100;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject aerialEplosionPrefab;
    [SerializeField] private LayerMask explosionLayerMask;
    private GameObject explosionParticles;
    private GameObject aerialExplosionParticles;
    private Vector3 savedPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        explosionParticles = Instantiate(explosionPrefab, Vector3.zero, Quaternion.identity);
        aerialExplosionParticles = Instantiate(aerialEplosionPrefab, Vector3.zero, Quaternion.identity);
        explosionParticles.SetActive(false);
        aerialExplosionParticles.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (transform.position.y < outOfBoundsY)
        {
            bulletCollision(false);
        }

        if (transform.localPosition != Vector3.zero)
        {
            savedPos = transform.position;
        }

        if (transform.parent == null)
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

        if (other.gameObject.layer == 8 && other.CompareTag(Constants.Tags.triggerCol))
        {
            aerialExplosionParticles.transform.position = savedPos;
            aerialExplosionParticles.SetActive(false);
            aerialExplosionParticles.SetActive(true);
            bulletCollision(true);
        }
        else if(other.gameObject.layer != 8)
        {
            explosionParticles.transform.position = savedPos;
            explosionParticles.SetActive(false);
            explosionParticles.SetActive(true);
            bulletCollision(false);
        }        
    }
}
