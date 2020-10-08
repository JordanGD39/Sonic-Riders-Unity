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
    [SerializeField] private GameObject raceButton;
    [SerializeField] private ButtonSounds sound;
    private bool alreadyStarted = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        fadePanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        selectionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !alreadyStarted)
        {
            alreadyStarted = true;
            sound.Pressed();
            anim.Play("FadeIn");
            Invoke("RemoveMainMenuPanel", 0.5f);
        }

        if (alreadyStarted && Input.GetButtonDown("Cancel"))
        {
            alreadyStarted = false;
            sound.Cancel();
            anim.Play("FadeIn");
            Invoke("RemoveSelectPanel", 0.5f);
        }
    }

    private void RemoveMainMenuPanel()
    {
        mainMenuPanel.SetActive(false);
        selectionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(raceButton);
    }

    private void RemoveSelectPanel()
    {
        selectionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadCharacterSelect()
    {
        GameManager.instance.LoadScene("CharacterSelect", false);
    }
}
