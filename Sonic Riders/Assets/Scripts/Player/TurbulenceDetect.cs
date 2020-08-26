using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbulenceDetect : MonoBehaviour
{
    private ParticleSystem ps;
    private GameObject parentPlayer;
    [SerializeField] private LayerMask layerMask;
    private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    private List<Collider> playerColliders = new List<Collider>();

    public List<Collider> PlayerColliders { get { return playerColliders; } }

    [SerializeField] private GameObject turbulenceRiderPref; 

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        parentPlayer = GetComponentInParent<PlayerMovement>().gameObject;

        List<GameObject> players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

        players.Remove(parentPlayer);

        for (int i = 0; i < players.Count; i++)
        {
            playerColliders.Add(players[i].GetComponentInChildren<PlayerTrigger>().GetComponent<CapsuleCollider>());
            ps.trigger.SetCollider(i, playerColliders[i].transform);            
        }
    }

    private void OnParticleTrigger()
    {
        //Debug.Log("GDGDWUG");
        int enterCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter);

        //Debug.Log(enterCount);

        for (int i = 0; i < enterCount; i++)
        {
            ParticleSystem.Particle p = enter[i];

            Collider[] colliders = Physics.OverlapSphere(p.position, 5, layerMask);            

            for (int j = 0; j < colliders.Length; j++)
            {
                //Debug.Log(colliders[i]);

                if (colliders[i].transform != transform.parent.parent.parent && colliders[i].isTrigger && playerColliders.Contains(colliders[i]))
                {
                    Debug.Log(colliders[i].gameObject.name);

                    playerColliders.Remove(colliders[i]);

                    PlayerMovement playerMovement = colliders[i].GetComponentInParent<PlayerMovement>();
                    playerMovement.CantMove = true;
                    Transform player = playerMovement.transform;

                    GameObject turbulenceRider = Instantiate(turbulenceRiderPref, p.position + player.TransformDirection(new Vector3(0, 4, 0)), Quaternion.identity);
                    player.parent = turbulenceRider.transform;

                    turbulenceRider.GetComponent<RideTurbulence>().ReadyToStart(p, this, player, ps);                    
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (enter.Count > 0)
        {
            for (int i = 0; i < enter.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(enter[i].position, 5);
            }
        }
    }
}
