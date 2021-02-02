using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BigCanvasUI : SurvivalFunctionsUI
{
    [SerializeField] private GameObject characterPlacePref;
    [SerializeField] private Transform characterPlaceParent;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject distanceRadar;
    public GameObject DistanceRadar { get { return distanceRadar; } }
    [SerializeField] private Animator tutorialAnim;
    [SerializeField] private Text tutorialText;

    [SerializeField] private Text survivalWin;
    private float timer = 0;
    private bool stopCounting = false;

    private ChangePlace changePlace;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        survivalWin.gameObject.SetActive(false);
        tutorialPanel.SetActive(false);
        changePlace = FindObjectOfType<ChangePlace>();
        characterPlaceParent.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        distanceRadar.SetActive(false);

        ReadyToChange = true;
    }

    private void Update()
    {
        if (stopCounting)
        {
            return;
        }

        timer += Time.deltaTime;

        timeText.text = TimerCalc(timer);
    }

    private string TimerCalc(float aTimer)
    {
        string minutes = Mathf.Floor(aTimer / 60).ToString("00");
        string seconds = Mathf.Floor(aTimer % 60).ToString("00");
        float centiseconds = aTimer * 100;

        centiseconds = centiseconds % 100;

        return minutes + "'" + seconds + "''" + centiseconds.ToString("00");
    }

    public void ShowTutorialText(string text)
    {        
        tutorialPanel.SetActive(true);
        tutorialText.text = text;
    }

    public void RemovePopup()
    {
        tutorialAnim.Play("PopupAway");

        StartCoroutine("DisablePopup");
    }

    private IEnumerator DisablePopup()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        tutorialPanel.SetActive(false);
    }

    public void PostPlacings(List<PlayerCheckpoints> players)
    {
        changePlace = gameObject.AddComponent<ChangePlace>();

        stopCounting = true;
        characterPlaceParent.gameObject.SetActive(true);

        for (int i = 0; i < players.Count; i++)
        {
            GameObject playerPlace = Instantiate(characterPlacePref, characterPlaceParent, false);

            playerPlace.GetComponent<RectTransform>().anchoredPosition = new Vector2(410 + (i * 20), -190 - (i * 50));

            CharacterPlaceReferences references = playerPlace.GetComponent<CharacterPlaceReferences>();

            Transform placingParent = references.PlacingParent;

            changePlace.ChangeImageReferences(placingParent.GetChild(1).GetComponent<Image>(), placingParent.GetChild(2), placingParent.GetChild(0).gameObject, i);
            changePlace.UpdatePlacing();

            CharacterStats stats = players[i].CharStats;

            references.Portrait.sprite = stats.Portrait;

            RectTransform portraitTransform = references.Portrait.GetComponent<RectTransform>();
            portraitTransform.anchoredPosition = new Vector2(portraitTransform.anchoredPosition.x, portraitTransform.anchoredPosition.y + stats.ExtraY);

            references.CharacterName.text = stats.CharacterName;

            references.TimeText.text = TimerCalc(stats.Timer);
        }

        changePlace.CantUpdate = true;
        StartCoroutine("ShowMenuPanel");
        StartCoroutine("ShowMenuPanelInput");
    }

    public void ShowSurvivalWinner(string charName)
    {
        stopCounting = true;
        GameManager.instance.GetAudioManager.StopPlaying(GameManager.instance.GetAudioManager.CurrSound.name);
        GameManager.instance.GetAudioManager.Play("Victory");
        survivalWin.gameObject.SetActive(true);
        survivalWin.text = charName.ToUpper() + " WINS!";

        StartCoroutine("ShowMenuPanel");
    }    

    private IEnumerator ShowMenuPanelInput()
    {
        while (!winPanel.activeSelf && !Input.GetButtonDown("Submit"))
        {
            yield return null;
        }

        SelectRestartButton();
    }

    private IEnumerator ShowMenuPanel()
    {
        yield return new WaitForSeconds(3);

        if (!winPanel.activeSelf)
        {
            winPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(restartButton);
        }        
    }

    private void SelectRestartButton()
    {
        winPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }

    public void PauseToggle(AudioManagerHolder audioHolder)
    {
        if (winPanel.activeSelf || survivalWin.gameObject.activeSelf)
        {
            return;
        }      

        pausePanel.SetActive(!pausePanel.activeSelf);

        if (GameManager.instance.GetAudioManager.CurrAudio.name != "Invincibility")
        {
            GameManager.instance.GetAudioManager.CurrSound.source.volume = pausePanel.activeSelf ? 0.3f : GameManager.instance.GetAudioManager.CurrSound.volume;
        }
        else
        {
            PauseAudioOnPause(GameManager.instance.GetAudioManager);
        }

        PauseAudioOnPause(audioHolder.SfxManager);
        PauseAudioOnPause(audioHolder.VoiceManager);

        Time.timeScale = pausePanel.activeSelf ? 0 : 1;

        if (pausePanel.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
    }

    private void PauseAudioOnPause(AudioManager audioManager)
    {
        if (audioManager.CurrAudio == null)
        {
            return;
        }

        if (pausePanel.activeSelf)
        {
            audioManager.Pause(audioManager.CurrAudio.name);
        }
        else
        {
            audioManager.UnPause(audioManager.CurrAudio.name);
        }
    }

    public void Restart()
    {
        if (!GameManager.instance.LoadingScene)
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameManager.instance.LoadScene(GameManager.instance.CurrScene, true);
        }        
    }

    public void LoadDifferentScene(string scene)
    {
        if (!GameManager.instance.LoadingScene)
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameManager.instance.LoadScene(scene, true);
        }
    }
}
