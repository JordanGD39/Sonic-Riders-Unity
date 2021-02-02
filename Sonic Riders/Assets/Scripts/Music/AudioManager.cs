using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public CharacterStats CharStats { get; set; }

    [SerializeField] private AudioMixerGroup mixer;
    
    private Sound currLoopingSound;

    private Sound currS;

    public Sound CurrSound { get { return currLoopingSound; } }
    public Sound CurrAudio { get { return currS; } }

    private bool fadeIn = false;

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
            s.source.priority = s.priority;
        }
    }

    public void StartFadeVoid(bool aFadeIn)
    {
        fadeIn = aFadeIn;
        StopCoroutine("StartFade");
        StartCoroutine("StartFade");
    }

    private IEnumerator StartFade()
    {
        if (currLoopingSound != null)
        {
            if (!fadeIn)
            {
                while (currLoopingSound.source.volume > 0)
                {
                    currLoopingSound.source.volume -= FadeOutRate;
                    yield return null;
                }

                currLoopingSound.source.Stop();
            }
            else
            {
                currLoopingSound.source.Play();
                while (currLoopingSound.source.volume < currLoopingSound.volume)
                {
                    currLoopingSound.source.volume += FadeOutRate;
                    yield return null;
                }
            }                   
        }
    }

    public void Play(string name)
    {
        CheckSound(name);

        if (currS == null)
        {
            return;
        }

        currS.source.timeSamples = 0;
        currS.source.pitch = currS.pitch;

        PlayCurrentSound();
    }

    public void PlayReverse(string name)
    {
        CheckSound(name);

        if (currS == null)
        {
            return;
        }

        currS.source.timeSamples = currS.clip.samples - 1;
        currS.source.pitch = -currS.pitch;

        PlayCurrentSound();
    }

    private void CheckSound(string name)
    {
        if (CharStats != null && !CharStats.IsPlayer)
        {
            return;
        }

        FindSound(name);

        if (currS == null)
        {
            return;
        }

        if (currS.loop)
        {
            currLoopingSound = currS;
            //Debug.Log("Playing music " + name);
        }
    }

    private void PlayCurrentSound()
    {
        currS.source.Play();
    }

    public void StopPlaying(string sound)
    {
        FindSound(sound);
        if (currS == null)
        {
            return;
        }

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
