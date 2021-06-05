using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperAI : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private List<PlayerMovement> players = new List<PlayerMovement>();
    [SerializeField] private int currTarget = -1;
    [SerializeField] private float rotateSpeed = 5;
    [SerializeField] private float slowdownSpeed = 8;
    [SerializeField] private float approachRate = 3;
    [SerializeField] private float attackHeight = 7;
    [SerializeField] private float lerpSpeed = 7;
    [SerializeField] private float distanceToPlayer = 5;
    [SerializeField] private GameObject chopper;
    [SerializeField] private GameObject sub;
    private Rigidbody rb;
    private Vector3 targetPos;
    private Vector3 lookPos;
    private bool backUnderWater = false;
    private bool aboveWater = false;
    private bool move = false;
    private bool delaySearch = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.FishPhobia)
        {
            chopper.SetActive(false);
            sub.SetActive(true);
        }

        rb = GetComponent<Rigidbody>();        
        transform.position = new Vector3(0, -50, 0);
    }

    public void SearchPlayers()
    {
        players.AddRange(FindObjectsOfType<PlayerMovement>());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > attackHeight)
        {
            transform.position = new Vector3(transform.position.x, attackHeight, transform.position.z);
        }

        if (currTarget < 0)
        {
            if (backUnderWater)
            {
                rb.isKinematic = false;
                move = true;
                Vector3 underSeaPos = new Vector3(0,-50, 0);

                if (transform.position.y > -50)
                {
                    lookPos = underSeaPos;
                    LookAtTarget();                    
                }
                else
                {
                    rb.isKinematic = true;
                    move = false;
                    backUnderWater = false;
                }
            }

            if (!delaySearch)
            {
                SearchForPlayerOffRoadWater();
            }
        }
        else
        {
            if (delaySearch)
            {
                TargetValid();

                AttackCheck(true);

                if (transform.localPosition.z < -distanceToPlayer)
                {
                    currTarget = -1;
                    transform.SetParent(null);
                    delaySearch = false;
                }

                return;
            }

            if (!aboveWater)
            {
                lookPos = players[currTarget].transform.position;
                LookAtTarget();
                GoAboveSea();
            }
            else
            {
                if (transform.position.y != attackHeight)
                {
                    transform.position = new Vector3(transform.position.x, attackHeight, transform.position.z);
                }

                lookPos = targetPos;

                TargetValid();

                if (currTarget < 0)
                {
                    return;
                }

                AttackCheck(false);
            }            
        }
    }

    private void LookAtTarget()
    {        
        Quaternion targetRotation = Quaternion.LookRotation(lookPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        //transform.LookAt(lookPos);
    }

    private void GoAboveSea()
    {
        Vector3 aboveSeaPos = transform.position;
        aboveSeaPos.y = attackHeight;
        transform.position = Vector3.Lerp(transform.position, aboveSeaPos, lerpSpeed * Time.deltaTime);

        if (transform.position.y >= attackHeight - 0.1f)
        {            
            transform.forward = transform.parent.forward;
            aboveWater = true;
        }
    }

    private void TargetValid()
    {
        PlayerMovement player = players[currTarget];

        if (!player.OnWater || player.OnTrack || Mathf.Abs(player.Speed) < 20 || player.Attacked)
        {
            TargetInvalid(false);
        }
    }

    private void TargetInvalid(bool delay)
    {
        if (delay)
        {
            delaySearch = true;
            Invoke("RemoveDelay", 2);
        }
        else
        {
            currTarget = -1;
            transform.SetParent(null);
        }

        backUnderWater = true;
        aboveWater = false;    
    }

    private void RemoveDelay()
    {
        delaySearch = false;
    }

    private void AttackCheck(bool back)
    {
        Vector3 approach = new Vector3(0, 0, approachRate * Time.deltaTime);

        transform.localPosition += back ? -approach : approach;

        if (transform.localPosition.z > 0)
        {
            TargetInvalid(true);
        }
    }

    private void SearchForPlayerOffRoadWater()
    {
        int target = -1;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].OnWater && !players[i].OnTrack && Mathf.Abs(players[i].Speed) > 20 && players[i].transform.position.y >= attackHeight && !players[i].Attacked)
            {
                target = i;
                break;
            }
        }

        if (target < 0)
        {
            return;
        }

        rb.isKinematic = true;
        backUnderWater = false;
        transform.SetParent(players[target].transform.GetChild(0));
        transform.forward = players[target].transform.GetChild(0).forward;
        transform.localPosition = new Vector3(0, transform.localPosition.y, -distanceToPlayer);
        currTarget = target;
    }

    private void FixedUpdate()
    {
        if (move)
        {
            rb.velocity = transform.forward * speed;
        }
    }
}
