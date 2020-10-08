using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void Select()
    {
        audioManager.Play("ButtonSelect");
    }

    public void Pressed()
    {
        audioManager.Play("ButtonPressed");
    }

    public void Cancel()
    {
        audioManager.Play("Cancel");
    }
}
