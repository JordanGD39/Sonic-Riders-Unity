using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    [SerializeField] private AudioMixerGroup mixer;
    
    private Sound currSound;

    private Sound currS;

    public Sound CurrSound { get { return currSound; } }

    public bool FadeOut { get; set; } = false;

    public float FadeOutRate { get; set; } = 0.001f;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = mixer;
            s.source.spatialBlend = s.spatialBlend;
            s.source.minDistance = s.minRange;
            s.source.maxDistance = s.maxRange;
        }
    }

    public IEnumerator StartFadeOut()
    {
        if (currSound != null)
        {
            FadeOut = true;

            while (currSound.volume > 0 && FadeOut)
            {
                currSound.source.volume -= FadeOutRate;
                yield return null;
            }

            if (FadeOut)
            {
                currSound.source.Stop();
                FadeOut = false;
            }            
        }
    }

    public void Play(string name)
    {
        Debug.Log("Searching for music " + name);
        FindSound(name);

        if (currS.loop)
        {
            currSound = currS;
            Debug.Log("Playing music " + name);
        }

        if (!currS.source.isPlaying)
        {
            currS.source.Play();
        }
    }

    public void StopPlaying(string sound)
    {
        FindSound(sound);
        currS.source.Stop();
    }

    public void Pause(string sound)
    {
        FindSound(sound);

        currS.source.Pause();
    }

    public void UnPause(string sound)
    {
        FindSound(sound);

        currS.source.UnPause();
    }

    public void Volume(string sound, float volume)
    {
        FindSound(sound);

        currS.source.volume = volume;
    }

    public Sound FindSound(string sound)
    {
        currS = null;
        currS = Array.Find(sounds, item => item.name == sound);

        if (currS == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return null;
        }

        return currS;
    }
}
