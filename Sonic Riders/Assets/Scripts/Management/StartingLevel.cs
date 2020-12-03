using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingLevel : MonoBehaviour
{
    private Transform psParent;
    private List<Text> countdownTexts = new List<Text>();

    [SerializeField] private bool noStart = false;

    private bool startCountDown = false;
    private float timer = 5;
    public float Timer { get { return timer; } }

    private bool doneCounting = false;

    [SerializeField] private TutorialManager tutorialManager;
    private SurvivalManager survivalManager;

    // Start is called before the first frame update
    void Start()
    {
        survivalManager = FindObjectOfType<SurvivalManager>();

        if (GameManager.instance.GetComponent<TestHandleJoin>() == null)
        {
            List<GameObject> playersInScene = new List<GameObject>();
            playersInScene.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

            for (int i = 0; i < playersInScene.Count; i++)
            {
                playersInScene[i].SetActive(false);
                playersInScene[i].tag = "Untagged";
            }

            GameManager.instance.GetComponent<PlayerConfigManager>().SpawnPlayers(this);
        }       

        psParent = GetComponentInChildren<ParticleSystem>().transform.parent;

        List<GameObject> texts = new List<GameObject>();
        texts.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.countdown));

        RaceManager raceManager = GameObject.FindGameObjectWithTag(Constants.Tags.raceManager).GetComponent<RaceManager>();

        if (raceManager != null)
        {
            raceManager.AddPlayers();
        }

        for (int i = 0; i < texts.Count; i++)
        {
            countdownTexts.Add(texts[i].GetComponent<Text>());
        }

        for (int i = 0; i < countdownTexts.Count; i++)
        {
            countdownTexts[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!startCountDown)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < -1)
        {
            psParent.gameObject.SetActive(false);
            enabled = false;
        }

        if (doneCounting)
        {
            return;
        }

        for (int i = 0; i < countdownTexts.Count; i++)
        {
            countdownTexts[i].text = timer.ToString("F2");
        }

        if (timer <= 0)
        {
            doneCounting = true;

            for (int i = 0; i < psParent.childCount; i++)
            {
                psParent.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }

            for (int i = 0; i < countdownTexts.Count; i++)
            {
                countdownTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlacePlayersInOrder()
    {
        ChopperAI chopper = FindObjectOfType<ChopperAI>();

        if (chopper != null)
        {
            chopper.SearchPlayers();
        }

        if (noStart)
        {
            for (int i = 0; i < countdownTexts.Count; i++)
            {
                countdownTexts[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < psParent.childCount; i++)
            {
                psParent.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }

            psParent.gameObject.SetActive(false);

            timer = -1;

            return;
        }

        List<GameObject> playersInScene = new List<GameObject>();
        playersInScene.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

        for (int i = 0; i < playersInScene.Count; i++)
        {
            if (i > 4)
            {
                i = 4 - i;
            }

            float x = i * 3;

            playersInScene[i].transform.position = new Vector3(x, transform.position.y + 0.4f, 0);
        }        

        if (survivalManager != null)
        {
            survivalManager.GetPlayers(playersInScene);
        }
        else
        {
            if (GameManager.instance.GameMode == GameManager.gamemode.TUTORIAL)
            {
                HUD hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform.GetChild(0).GetComponent<HUD>();
                hud.DistanceRadar.SetActive(false);
                hud.PlacingUI.SetActive(false);
                BigCanvasUI bigCanvas = FindObjectOfType<BigCanvasUI>();
                bigCanvas.DistanceRadar.SetActive(false);
            }
        }

        if (tutorialManager == null)
        {
            Invoke("StartCountdown", 0.5f);
        }
        else
        {
            tutorialManager.StartLevel = this;
            tutorialManager.GivePlayerComponents(playersInScene[0].GetComponent<PlayerMovement>());
        }
    }

    public void StartCountDownTutorial()
    {
        Invoke("StartCountdown", 0.5f);
    }

    private void StartCountdown()
    {
        for (int i = 0; i < countdownTexts.Count; i++)
        {
            countdownTexts[i].gameObject.SetActive(true);
        }

        startCountDown = true;
    }
}
