using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackSelectInput : MonoBehaviour
{
    private ButtonSounds buttonSounds;
    [SerializeField] private GameObject button;

    private void Start()
    {
        buttonSounds = GetComponentInChildren<ButtonSounds>();
        Invoke("SetTrackButtonSelected", 2);
    }

    private void SetTrackButtonSelected()
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.LoadingScene && Input.GetButtonDown("Cancel"))
        {
            EventSystem.current.SetSelectedGameObject(null);
            buttonSounds.Cancel();
            GameManager.instance.LoadScene("MainMenu", false);
        }
    }
}
