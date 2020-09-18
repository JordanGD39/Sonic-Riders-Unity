using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerBasedOnLevel : MonoBehaviour
{
    [SerializeField] private string musicName;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.GetAudioManager.CurrSound != null)
        {
            GameManager.instance.GetAudioManager.StopPlaying(GameManager.instance.GetAudioManager.CurrSound.name);
        }
        GameManager.instance.GetAudioManager.Play(musicName);
    }
}
