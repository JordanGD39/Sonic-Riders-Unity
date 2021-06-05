using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TrackButton : MonoBehaviour
{
    [SerializeField] private string trackName;
    [SerializeField] private string shownTrackName;
    [SerializeField] private TextMeshProUGUI trackNameText;

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
