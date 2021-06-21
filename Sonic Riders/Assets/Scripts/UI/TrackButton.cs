using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TrackButton : MonoBehaviour
{
    [SerializeField] private string trackName;
    [SerializeField] private string shownTrackName;
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private bool selectable = true;
    [SerializeField] private GameObject nonSelectableImage;

    private void Start()
    {
        if (!selectable && GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            GetComponent<Button>().interactable = false;
            nonSelectableImage.SetActive(true);
        }
    }

    public void GoToCharacterSelect()
    {
        if (!GameManager.instance.LoadingScene)
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameManager.instance.TrackToLoad = trackName;
            GameManager.instance.LoadScene("CharacterSelect", false);
        }        
    }

    public void ShowTrackName()
    {
        trackNameText.text = shownTrackName;
    }
}
