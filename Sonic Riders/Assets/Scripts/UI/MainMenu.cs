using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject raceButton;
    [SerializeField] private GameObject qualityButton;
    [SerializeField] private ButtonSounds sound;
    private bool alreadyStarted = false;
    private bool showOptions = false;
    private bool fading = false;

    private void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 5));
        anim = GetComponent<Animator>();
        fadePanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        selectionPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit") && !alreadyStarted)
        {
            sound.Pressed();
            RemovePanel("RemoveMainMenuPanel", true, false);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (alreadyStarted)
            {
                if (optionsPanel.activeSelf)
                {
                    Options(false);
                }
                else
                {
                    RemovePanel("RemoveSelectPanel", false, true);
                }
            }
            else
            {
                Application.Quit();
            }            
        }
    }

    private void RemovePanel(string function, bool started, bool cancelSound)
    {
        if (fading)
        {
            return;
        }

        fading = true;

        alreadyStarted = started;

        if (cancelSound)
        {
           sound.Cancel();
        }

        anim.Play("FadeIn");
        
        Invoke(function, 0.5f);
        Invoke("StopFading", 0.5f);
    }

    private void StopFading()
    {
        fading = false;
    }

    private void RemoveMainMenuPanel()
    {
        mainMenuPanel.SetActive(false);
        selectionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(raceButton);
    }

    private void OptionsPanel()
    {
        selectionPanel.SetActive(!showOptions);
        optionsPanel.SetActive(showOptions);

        if (showOptions)
        {
            EventSystem.current.SetSelectedGameObject(qualityButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(raceButton);
        }
    }

    public void Options(bool show)
    {
        showOptions = show;
        RemovePanel("OptionsPanel", true, !show);
    }

    private void RemoveSelectPanel()
    {
        selectionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadCharacterSelect(int mode)
    {
        if (GameManager.instance.LoadingScene)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);

        GameManager.instance.GameMode = (GameManager.gamemode)mode;

        if (GameManager.instance.GameMode != GameManager.gamemode.TUTORIAL)
        {
            GameManager.instance.LoadScene("TrackSelect", false);
        }
        else
        {
            GameManager.instance.TrackToLoad = "Tutorial";
            GameManager.instance.LoadScene("CharacterSelect", false);
        }

    }
}
