using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;

public class SurvivalManager : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    private Transform raceCheckpointsParent;
    [SerializeField] private Transform emerald;
    private ChaosEmerald chaosEmerald;
    [SerializeField] private GameObject leader;
    private int leaderIndex = 0;
    private PlayerCheckpoints currPlayerCheckpoints;
    private CharacterStats leaderStats;
    public GameObject Leader { set { leader = value; } }

    [SerializeField] private Transform scoreRingsParent;
    private List<GameObject> scoreRings = new List<GameObject>();
    private List<GameObject> disabledScoreRings = new List<GameObject>();

    private List<HUD> huds = new List<HUD>();
    private BigCanvasUI bigCanvasUI;

    private List<GameObject> players = new List<GameObject>();
    private List<PlayerCheckpoints> playersCheckpoints = new List<PlayerCheckpoints>();
    [SerializeField] private float[] distances = { 0,0,0,0 };
    [SerializeField] private float[] distancesX = { 0,0,0,0 };
    [SerializeField] private float distanceMultiplier = 1.5f;
    [SerializeField] private float distanceXMultiplier = 1;
    private int playerCount = 0;

    private float outOfRadar = 140;

    private void Start()
    {
        chaosEmerald = emerald.GetComponent<ChaosEmerald>();
        bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();

        raceCheckpointsParent = GameObject.FindGameObjectWithTag(Constants.Tags.raceManager).transform;

        for (int i = 0; i < scoreRingsParent.childCount; i++)
        {
            scoreRings.Add(scoreRingsParent.GetChild(i).gameObject);
        }

        outOfRadar /= distanceMultiplier;
    }

    // Start is called before the first frame update
    public void GetPlayers(List<GameObject> somePlayers)
    {
        playerCount = GameManager.instance.GetComponent<PlayerInputManager>().playerCount;

        if (playerCount == 0)
        {
            return;
        }

        huds.Clear();

        Transform canvasParent = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform;

        for (int i = 0; i < playerCount; i++)
        {
            huds.Add(canvasParent.GetChild(i).GetComponent<HUD>());
        }

        if (GameManager.instance.GameMode != GameManager.gamemode.SURVIVAL)
        {
            if (playerCount <= 2)
            {
                for (int i = 0; i < huds.Count; i++)
                {
                    huds[i].DistanceRadar.SetActive(false);
                }
            }
            else
            {
                bigCanvasUI.DistanceRadar.SetActive(false);
            }

            gameObject.SetActive(false);
            return;
        }

        players = somePlayers;
        players.Sort(CompareIndex);

        for (int i = 0; i < players.Count; i++)
        {
            playersCheckpoints.Add(players[i].GetComponent<PlayerCheckpoints>());
        }        

        StartCoroutine("WaitForUI");      
    }

    private IEnumerator WaitForUI()
    {
        if (playerCount <= 2)
        {
            while (!huds[huds.Count - 1].ReadyToChange)
            {
                yield return null;
            }
        }
        else
        {
            while (!bigCanvasUI.ReadyToChange)
            {
                yield return null;
            }
        }

        ReadyToChangeUI();
    }

    private void ReadyToChangeUI()
    {
        if (playerCount <= 2)
        {
            for (int i = 0; i < playerCount; i++)
            {
                for (int j = 0; j < playerCount; j++)
                {
                    huds[i].ChangeSurvivalColor(j, players[j].GetComponent<CharacterStats>().CharColor);
                    huds[i].ChangeIcons(j, players[j].GetComponent<CharacterStats>().Icon);
                    huds[i].ShowSurvivalScores(j);
                }
            }

            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].DistanceRadar.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].DistanceRadar.SetActive(false);
            }

            if (huds.Count > 2)
            {
                huds[0].ReverseAirBar();
                huds[2].ReverseAirBar();
            }

            for (int i = 0; i < playerCount; i++)
            {
                bigCanvasUI.ChangeSurvivalColor(i, players[i].GetComponent<CharacterStats>().CharColor);
                bigCanvasUI.ChangeIcons(i, players[i].GetComponent<CharacterStats>().Icon);
                bigCanvasUI.ShowSurvivalScores(i);
            }

            bigCanvasUI.DistanceRadar.SetActive(true);
        }
    }

    private int CompareIndex(GameObject a, GameObject b)
    {
        return a.GetComponent<CharacterStats>().PlayerIndex.CompareTo(b.GetComponent<CharacterStats>().PlayerIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (huds.Count == 0 && bigCanvasUI == null)
        {
            return;
        }

        float emeraldClosestDistance = 1 - path.path.GetClosestTimeOnPath(emerald.position);

        for (int i = 0; i < players.Count; i++)
        {
            if (i != leaderIndex || leader == null)
            {
                float vel = 0;

                float calcDistance = CalculateDistanceToEmerald(playersCheckpoints[i]);

                if ((calcDistance >= -outOfRadar && distances[i] <= outOfRadar) || (distances[i] >= -outOfRadar && calcDistance <= outOfRadar))
                {
                    distances[i] = Mathf.SmoothDamp(distances[i], calcDistance, ref vel, 0.1f);
                }
                else
                {
                    distances[i] = calcDistance;
                }
                
            }
            
            //float playerClosestDistance = 1 - path.path.GetClosestTimeOnPath(players[i].transform.position);

            /*float halfPoint = playerClosestDistance + 0.5f;

            if (emeraldClosestDistance > halfPoint)
            {
                emeraldClosestDistance = 1 - emeraldClosestDistance;
            }*/

            //if (playerClosestDistance > 0.5f && emeraldClosestDistance < 0.5f)
            //{
            //playerClosestDistance -= 1;
            //}

            //distances[i] = playerClosestDistance - emeraldClosestDistance;
            /*
            distances[i] = Vector3.Dot(emerald.forward, players[i].transform.position - emerald.position);
            distancesX[i] = Vector3.Dot(emerald.right, players[i].transform.position - emerald.position);
            */
        }

        if (playerCount < 3)
        {
            for (int i = 0; i < huds.Count; i++)
            {
                for (int j = 0; j < playerCount; j++)
                {
                    huds[i].UpdateIconDistance(j, distances[j] * distanceMultiplier, distancesX[j] * distanceXMultiplier, distanceXMultiplier);
                }
            }
        }
        else
        {
            for (int i = 0; i < huds.Count; i++)
            {
                bigCanvasUI.UpdateIconDistance(i, distances[i] * distanceMultiplier, distancesX[i] * distanceXMultiplier, distanceXMultiplier);
            }
        }
    }

    private float CalculateDistanceToEmerald(PlayerCheckpoints player)
    {
        if (player.CurrSurvivalCheckpoint == chaosEmerald.Checkpoint)
        {
            if (Vector3.Dot(emerald.forward, player.transform.position - emerald.position) > 0)
            {
                return (emerald.position - player.transform.position).sqrMagnitude;
            }
            else
            {
                return -(emerald.position - player.transform.position).sqrMagnitude;
            }

        }

        int maxCheckpoints = raceCheckpointsParent.transform.childCount - 1;
        int diffCheckpoints = Mathf.Abs(chaosEmerald.Checkpoint - player.CurrSurvivalCheckpoint);        

        float distancesCombined = 0;

        bool pastEmerald = false;

        for (int i = 1; i < diffCheckpoints + 1; i++)
        {
            int checkpoint = 0;

            if (player.CurrSurvivalCheckpoint > chaosEmerald.Checkpoint)
            {
                checkpoint = player.CurrCheckpoint - i;
                pastEmerald = true;
            }
            else
            {
                checkpoint = player.CurrCheckpoint + i;
            }

            if (checkpoint > maxCheckpoints)
            {
                checkpoint -= maxCheckpoints;
            }
            else if (checkpoint < 0)
            {
                checkpoint += maxCheckpoints + 1;
            }

            float distance = (raceCheckpointsParent.GetChild(checkpoint).position - player.transform.position).sqrMagnitude;

            distancesCombined += distance;
        }

        if (pastEmerald)
        {
            return distancesCombined;
        }
        else
        {
            return -distancesCombined;
        }        
    }

    public void MakePlayerLeader(GameObject player)
    {
        if (leader != null)
        {
            leaderStats.SurvivalLeader = false;
        }

        leader = player;

        leaderIndex = players.IndexOf(leader);

        leaderStats = leader.GetComponent<CharacterStats>();

        leaderStats.SurvivalLeader = true;
        leaderStats.Air = leaderStats.MaxAir;
        currPlayerCheckpoints = leader.GetComponent<PlayerCheckpoints>();

        if (playerCount < 3)
        {
            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].UpdateLeader(leaderStats.Icon);
            }
        }
        else
        {
            bigCanvasUI.UpdateLeader(leaderStats.Icon);
        }
    }

    public void MoveChaosEmerald(GameObject player)
    {
        if (player != leader)
        {
            return;
        }

        Transform checkpoint = currPlayerCheckpoints.RaceManagerScript.transform.GetChild(currPlayerCheckpoints.CalcNextCheckpoint(2));

        Vector3 checkpointPos = checkpoint.GetChild(0).position;
        checkpointPos.y = 1.5f;
        emerald.SetParent(transform);
        chaosEmerald.FlyToPos(checkpointPos, checkpoint.forward);
        emerald.GetChild(0).localScale = new Vector3(2, 2, 2);       
        leader.GetComponentInChildren<PlayerTrigger>().Electrocute(2, false);
        leaderStats.SurvivalLeader = false;
        leader = null;

        if(playerCount < 3)
        {
            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].UpdateLeader(null);
            }
        }
        else
        {
            bigCanvasUI.UpdateLeader(null);
        }

        ResetDisabledScoreRings();
    }

    public void CheckValidScoreRing(GameObject scoreRing, GameObject player)
    {
        if (player != leader)
        {
            return;
        }

        if (scoreRings.Contains(scoreRing))
        {
            scoreRings.Remove(scoreRing);
            disabledScoreRings.Add(scoreRing);

            leaderStats.Air = leaderStats.MaxAir;
            leaderStats.SurvivalScore++;

            if (playerCount < 3)
            {
                for (int i = 0; i < huds.Count; i++)
                {
                    huds[i].UpdateSurvivalScore(players.IndexOf(leader), leaderStats.SurvivalScore);
                }
            }
            else
            {
                bigCanvasUI.UpdateSurvivalScore(players.IndexOf(leader), leaderStats.SurvivalScore);
            }            

            if (leaderStats.SurvivalScore >= 5)
            {
                Win();
            }
        }
    }

    public void Win()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<CharacterStats>().DisableAllFeatures = true;
        }

        bigCanvasUI.ShowSurvivalWinner(leaderStats.CharacterName);
    }

    public void CheckLeaderLap(GameObject player)
    {
        if (leader == player)
        {
            ResetDisabledScoreRings();
        }
    }

    private void ResetDisabledScoreRings()
    {
        for (int i = 0; i < disabledScoreRings.Count; i++)
        {
            scoreRings.Add(disabledScoreRings[i]);
        }

        disabledScoreRings.Clear();
    }
}
