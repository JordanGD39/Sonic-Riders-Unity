using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{
    private List<EnteringData> playersInRange = new List<EnteringData>();
    private List<PlayerPoleData> playersOnPole = new List<PlayerPoleData>();
    private List<PlayerPoleData> flyingTowardsTarget = new List<PlayerPoleData>();
    private List<Transform> playersFallingOff = new List<Transform>();
    [SerializeField] private Transform leftExitPoint;
    [SerializeField] private Transform rightExitPoint;
    [SerializeField] private Transform flyTarget;
    [SerializeField] private float lerpSpeed = 10;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float flySpeed = 1;
    [SerializeField] private float distanceToStop = 10;
    [SerializeField] private float speedLoss = 4;

    private void Update()
    {
        if (playersInRange.Count > 0)
        {
            CheckInRange();
        }

        if (playersOnPole.Count > 0)
        {
            MoveAllPlayersOnPole();
        }

        if (flyingTowardsTarget.Count > 0)
        {
            for (int i = 0; i < flyingTowardsTarget.Count; i++)
            {
                FlyTowardsTarget(flyingTowardsTarget[i]);
            }            
        }
    }

    private void CheckInRange()
    {
        for (int i = 0; i < playersInRange.Count; i++)
        {
            EnteringData enteringData = playersInRange[i];
            Transform player = enteringData.player;
            Vector3 pos = player.position - transform.position;
            float dot = Vector3.Dot(pos.normalized, transform.forward);
            float angle = Vector3.Angle(transform.forward, enteringData.aheadOfPole ? -player.forward : player.forward);

            //Debug.Log("Start ahead " + enteringData.aheadOfPole + " Dot: " + dot + " Angle: " + angle);
            bool lookingInDir = angle < 90;

            if (((!enteringData.aheadOfPole && dot > 0) || (enteringData.aheadOfPole && dot < 0)) && lookingInDir)
            {
                playersInRange.Remove(enteringData);
                AddPlayer(player.parent, enteringData.aheadOfPole);
            }
        }
    }

    private void MoveAllPlayersOnPole()
    {
        for (int i = 0; i < playersOnPole.Count; i++)
        {
            PlayerPoleData poleData = playersOnPole[i];
            Rigidbody rb = poleData.rb;

            rb.transform.Rotate(0, (poleData.rightSide ? -poleData.speed : poleData.speed) * Time.deltaTime, 0);
            poleData.hud.UpdateSpeedText(poleData.speed / 12);

            if (rb.transform.GetChild(0).localPosition != Vector3.zero)
            {
                MovePlayerToPole(poleData);
            }
            else
            {
                MovePlayerUpPole(poleData);
            }
        }
    }

    private void MovePlayerToPole(PlayerPoleData poleData)
    {
        Transform player = poleData.rb.transform.GetChild(0);

        player.localPosition = Vector3.MoveTowards(player.localPosition, Vector3.zero, lerpSpeed * Time.deltaTime);
    }

    private void MovePlayerUpPole(PlayerPoleData poleData)
    {
        Transform player = poleData.rb.transform;

        if (player.localPosition.y > poleData.targetExitPoint.parent.localPosition.y)
        {
            if (Vector3.Angle(player.transform.forward, poleData.targetExitPoint.forward) < 15)
            {
                playersOnPole.Remove(poleData);
                ResetPlayerData(poleData);
                player.GetChild(0).LookAt(flyTarget);
                player.GetComponent<PlayerTricks>().ChangeTrickSpeed(1, 0, 1, true);
                flyingTowardsTarget.Add(poleData);
            }
            return;
        }

        float moveSpeedPercent = poleData.speed / 800;

        if (moveSpeedPercent < 0.3f)
        {
            playersFallingOff.Add(poleData.rb.transform);
            playersOnPole.Remove(poleData);
            ResetPlayerData(poleData);
            ReleasePlayer(poleData);
            return;
        }

        float movingSpeed = moveSpeed * moveSpeedPercent;
        poleData.speed -= speedLoss * Time.deltaTime;        

        player.localPosition += Vector3.up * movingSpeed * Time.deltaTime;
        poleData.characterStats.Cam.localPosition = player.localPosition;
    }

    private void ResetPlayerData(PlayerPoleData poleData)
    {
        Transform player = poleData.rb.transform;

        player.SetParent(null);
        poleData.characterStats.Cam.SetParent(poleData.oldCamParent);
        player.rotation = new Quaternion(0, 0, 0, player.rotation.w);
        player.GetChild(0).GetChild(0).rotation = new Quaternion(0, 0, 0, 0);
        player.GetChild(0).GetChild(0).localPosition = Vector3.zero;
        poleData.characterStats.Cam.localPosition = Vector3.up * 1.5f;
        poleData.characterStats.DisableAllFeatures = false;
    }

    private void FlyTowardsTarget(PlayerPoleData poleData)
    {
        Transform player = poleData.rb.transform;

        Vector3 oldPos = player.position;

        player.position = Vector3.MoveTowards(player.position, flyTarget.position, flySpeed * Time.deltaTime);

        poleData.velocity = (player.position - oldPos) / Time.deltaTime;

        //Debug.Log(poleData.velocity);

        poleData.hud.UpdateSpeedText(poleData.velocity.magnitude);

        if ((player.position - flyTarget.position).sqrMagnitude < distanceToStop)
        {
            flyingTowardsTarget.Remove(poleData);
            ReleasePlayer(poleData);
        }
    }

    private void ReleasePlayer(PlayerPoleData poleData)
    {
        poleData.playerMovement.CantMove = false;
        poleData.rb.isKinematic = false;
        poleData.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //poleData.characterStats.DisableAllFeatures = false;
        poleData.characterStats.DontAlign = false;
        poleData.rb.transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
        poleData.rb.velocity = poleData.velocity;
    }

    private void AddPlayer(Transform player, bool cameFromBehind)
    {
        CharacterStats charStats = player.GetComponent<CharacterStats>();

        if (charStats.DisableAllFeatures)
        {
            return;
        }

        charStats.DisableAllFeatures = true;
        charStats.DontAlign = true;

        player.SetParent(transform);

        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;

        PlayerPoleData poleData = new PlayerPoleData();
        poleData.rb = rb;

        poleData.playerMovement = player.GetComponent<PlayerMovement>();
        poleData.playerMovement.CantMove = true;

        float speed = poleData.playerMovement.Speed * 12;

        if (speed < 250)
        {
            speed = 260;
        }

        poleData.speed = speed;
        poleData.rightSide = (player.localPosition.x > 0 && !cameFromBehind) || (player.localPosition.x < 0 && cameFromBehind);
        poleData.hud = charStats.Hud;
        poleData.targetPolePos = new Vector3(0, player.localPosition.y > 2 ? player.localPosition.y : 2, 0);
        poleData.characterStats = charStats;
        poleData.targetExitPoint = poleData.rightSide ? leftExitPoint : rightExitPoint;
        poleData.oldCamParent = charStats.Cam.parent;
        charStats.Cam.SetParent(transform);
        //charStats.Cam.localPosition = poleData.targetPolePos;

        Vector3 oldPos = player.localPosition;
        player.localPosition = poleData.targetPolePos;
        player.GetChild(0).localPosition = oldPos;

        Transform forwardChild = player.transform.GetChild(0).GetChild(0);

        forwardChild.LookAt(new Vector3(transform.position.x, forwardChild.position.y, transform.position.z));
        Vector3 forward = forwardChild.forward;
        forwardChild.forward = transform.forward;
        forwardChild.up = forward;

        forwardChild.localPosition = -forward * 1.5f;

        playersOnPole.Add(poleData);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform player = other.transform.root;

        if (!player.CompareTag("Player") || (playersFallingOff.Count > 0 && playersFallingOff.Contains(player)))
        {
            return;
        }

        EnteringData enterData = new EnteringData();
        enterData.player = player.GetChild(0);
        Vector3 pos = player.position - transform.position;
        enterData.aheadOfPole = Vector3.Dot(pos.normalized, transform.forward) > 0;

        playersInRange.Add(enterData);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform player = other.transform.root;

        if (!player.CompareTag("Player"))
        {
            return;
        }

        if (playersFallingOff.Count > 0 && playersFallingOff.Contains(player))
        {
            playersFallingOff.Remove(player);
        }

        for (int i = 0; i < playersInRange.Count; i++)
        {
            if (player.GetChild(0) == playersInRange[i].player)
            {
                playersInRange.RemoveAt(i);
            }
        }
    }
}

class PlayerPoleData
{
    public Rigidbody rb;
    public CharacterStats characterStats;
    public PlayerMovement playerMovement;
    public HUD hud;
    public float speed;
    public Vector3 velocity;
    public bool rightSide = true;
    public Transform targetExitPoint;
    public Vector3 targetPolePos;
    public Transform oldCamParent;
}

class EnteringData
{
    public Transform player;
    public bool aheadOfPole = false;
}
