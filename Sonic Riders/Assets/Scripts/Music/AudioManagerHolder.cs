using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerHolder : MonoBehaviour
{
    [SerializeField] private AudioManager voiceManager;
    public AudioManager VoiceManager { get { return voiceManager; } }

    [SerializeField] private AudioManager sfxManager;
    public AudioManager SfxManager { get { return sfxManager; } }

    private void Start()
    {
        CharacterStats charStats = GetComponent<CharacterStats>();
        voiceManager.CharStats = charStats;
        sfxManager.CharStats = charStats;
    }
}
