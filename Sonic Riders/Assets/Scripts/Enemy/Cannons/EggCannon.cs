using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCannon : MonoBehaviour
{
    private bool readyToFire = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform target;
    private Transform prevTarget;
    [SerializeField] private Transform barrel;
    [SerializeField] private float launchAngle = 70;
    [SerializeField] private float rotationSpeed = 60;
    [SerializeField] private float barrelRotationSpeed = 1;
    [SerializeField] private PlayersInRange playersInRange;
    private Vector3 targetPos;
    private float targetBarrelRotation;
    [SerializeField] private float targetVelocity = 0;
    private Vector3 targetOldPos;
    private float currentTargetXrotation;
    private GameObject bullet;
    private Rigidbody bulletRb;
    private EggBullet eggBullet;
    private bool launching = false;

    // Start is called before the first frame update
    void Start()
    {
        bullet = Instantiate(bulletPrefab, bulletSpawnPoint, false);
        bulletRb = bullet.GetComponent<Rigidbody>();
        eggBullet = bullet.GetComponent<EggBullet>();
        eggBullet.bulletCollision = BulletCollided;
        targetBarrelRotation = 360 - launchAngle;
        currentTargetXrotation = targetBarrelRotation;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        ResetBullet();
        StartCoroutine("RotateBarrel");
    }

    private void Update()
    {
        if (!readyToFire || playersInRange.PlayersInRangeList.Count == 0 || Time.timeScale == 0)
        {
            return;
        }

        if (!launching)
        {
            launching = true;
            Invoke("Launch", 1);
        }

        //Quaternion targetRotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z));
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator RotateBarrel()
    {
        while(Mathf.Round(barrel.localRotation.eulerAngles.x) != currentTargetXrotation)
        {
            //Debug.Log(barrel.localRotation.eulerAngles.x);
            barrel.localRotation = Quaternion.Lerp(barrel.localRotation, Quaternion.Euler(currentTargetXrotation, 0, 0), barrelRotationSpeed * Time.deltaTime);
            yield return null;
        }
        readyToFire = currentTargetXrotation > 0;
    }

    private void Launch()
    {
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(targetPos.x, 0.0f, targetPos.z);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        float H = target.position.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2 * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        bullet.transform.SetParent(null);
        bulletRb.isKinematic = false;
        bulletRb.velocity = globalVelocity;
        
        eggBullet.targetReady = true;        
    }

    private void BulletCollided()
    {
        ResetBullet();
        launching = false;
    }

    private void ResetBullet()
    {
        bullet.transform.SetParent(bulletSpawnPoint);
        eggBullet.targetReady = false;
        bulletRb.isKinematic = true;
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.localRotation = new Quaternion(0, 0, 0, bullet.transform.localRotation.w);        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPos, 2);
    }
}
