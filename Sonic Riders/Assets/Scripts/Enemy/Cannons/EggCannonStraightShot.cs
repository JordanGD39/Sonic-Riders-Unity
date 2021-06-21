using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCannonStraightShot : EggCannon
{
    [SerializeField] private float speed = 20;
    [SerializeField] private float damping = 20;
    [SerializeField] private bool noLookCalc = false;
    [SerializeField] private float extraAimHeight = 1;

    // Start is called before the first frame update
    protected override void Start()
    {
        readyToFire = true;
        base.Start();        
    }

    protected override void Update()
    {
        base.Update();

        if (!launching)
        {
            return;
        }

        int closestIndex = -1;

        if (playersInRange.PlayersInRangeList.Count == 1)
        {
            target = playersInRange.PlayersInRangeList[0].transform;
        }
        else
        {
            float dist = Mathf.Infinity;

            for (int i = 0; i < playersInRange.PlayersInRangeList.Count; i++)
            {
                Transform player = playersInRange.PlayersInRangeList[i].transform;
                float calcDist = (transform.position - player.position).sqrMagnitude;

                if (calcDist < dist)
                {
                    dist = calcDist;
                    target = player;
                    closestIndex = i;
                }
            }
        }

        if (target == null || closestIndex < 0)
        {
            return;
        }

       // barrel.LookAt(new Vector3(barrel.position.x, target.position.y, barrel.position.z));
        Vector3 lookPos = target.position + playersInRange.RigidBodiesInRange[closestIndex].velocity - barrel.transform.position;
        lookPos.y += extraAimHeight;
        Quaternion rotation = Quaternion.LookRotation(lookPos);

        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, rotation, Time.deltaTime * damping);

        barrel.transform.localRotation = new Quaternion(barrel.transform.localRotation.x, 0, 0, barrel.transform.localRotation.w);
        //Debug.Log(target.position.y);

        Quaternion barrelRot = barrel.localRotation;

        if (barrelRot.x < -0.5f)
        {
            barrelRot.x = -0.5f;
        }
        else if (barrelRot.x > 0)
        {
            barrelRot.x = 0;
        }

        barrel.localRotation = barrelRot;

        lookPos = target.position + playersInRange.RigidBodiesInRange[closestIndex].velocity - transform.position;
        lookPos.y = 0;
        rotation = Quaternion.LookRotation(lookPos);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    protected override void Launch()
    {
        bulletRb.useGravity = false;
        LaunchBullet(barrel.forward * speed);
    }
}
