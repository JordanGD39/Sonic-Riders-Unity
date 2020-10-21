using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SurvivalManager : MonoBehaviour
{
    [SerializeField] private Transform emerald;
    private ChaosEmerald chaosEmerald;
    [SerializeField] private GameObject leader;
    private PlayerCheckpoints currPlayerCheckpoints;
    private CharacterStats leaderStats;
    public GameObject Leader { set { leader = value; } }

    [SerializeField] private Transform scoreRingsParent;
    private List<GameObject> scoreRings = new List<GameObject>();
    private List<GameObject> disabledScoreRings = new List<GameObject>();

    private List<HUD> huds = new List<HUD>();
    private BigCanvasUI bigCanvasUI;

    private List<GameObject> players = new List<GameObject>();
    [SerializeField] private float[] distances = { 0,0,0,0 };
    [SerializeField] private float[] distancesX = { 0,0,0,0 };
    [SerializeField] private float distanceMultiplier = 1.5f;
    [SerializeField] private float distanceXMultiplier = 1;
    private int playerCount = 0;

    private void Start()
    {
        chaosEmerald = emerald.GetComponent<ChaosEmerald>();
        for (int i = 0; i < scoreRingsParent.childCount; i++)
        {
            scoreRings.Add(scoreRingsParent.GetChild(i).gameObject);
        }
    }

    // Start is called before the first frame update
    public void GetPlayers(List<GameObject> somePlayers)
    {
        playerCount = GameManager.instance.GetComponent<PlayerInputManager>().playerCount;

        if (playerCount == 0)
        {
            return;
        }

        bool notRightMode = GameManager.instance.GameMode != GameManager.gamemode.SURVIVAL;

        players = somePlayers;
        players.Sort(CompareIndex);

        if (playerCount < 3)
        {
            huds.Clear();
            Transform canvasParent = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform;
            for (int i = 0; i < playerCount; i++)
            {
                huds.Add(canvasParent.GetChild(i).GetComponent<HUD>());

                for (int j = 0; j < playerCount; j++)
                {
                    huds[i].ChangeSurvivalColor(j, players[j].GetComponent<CharacterStats>().CharColor);
                    huds[i].ChangeIcons(j, players[j].GetComponent<CharacterStats>().Icon);
                    huds[i].ShowSurvivalScores(j);
                }
            }

            if (notRightMode)
            {
                for (int i = 0; i < huds.Count; i++)
                {
                    huds[i].DistanceRadar.SetActive(false);
                }

                enabled = false;
                gameObject.SetActive(false);
                return;
            }

            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].DistanceRadar.SetActive(true);
            }
        }
        else
        {
            huds.Clear();
            huds.AddRange(FindObjectsOfType<HUD>());

            for (int i = 0; i < huds.Count; i++)
            {
                huds[i].DistanceRadar.SetActive(false);
            }

            if (notRightMode)
            {
                enabled = false;
                gameObject.SetActive(false);
                return;
            }

            bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();

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

        for (int i = 0; i < players.Count; i++)
        {
            distances[i] = Vector3.Dot(emerald.forward, players[i].transform.position - emerald.position);
            distancesX[i] = Vector3.Dot(emerald.right, players[i].transform.position - emerald.position);
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

    public void MakePlayerLeader(GameObject player)
    {
        if (leader != null)
        {
            leaderStats.SurvivalLeader = false;
        }

        leader = player;

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
        Debug.Log(leaderStats.CharacterName + " won the game!");
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
