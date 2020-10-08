using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public CharacterStats CharStats { get; set; }

    [SerializeField] private AudioMixerGroup mixer;
    
    private Sound currSound;

    private Sound currS;

    public Sound CurrSound { get { return currSound; } }

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
        if (currSound != null)
        {
            if (!fadeIn)
            {
                while (currSound.source.volume > 0)
                {
                    currSound.source.volume -= FadeOutRate;
                    yield return null;
                }

                currSound.source.Stop();
            }
            else
            {
                currSound.source.Play();
                while (currSound.source.volume < currSound.volume)
                {
                    currSound.source.volume += FadeOutRate;
                    yield return null;
                }
            }                   
        }
    }

    public void Play(string name)
    {
        if (CharStats != null && !CharStats.IsPlayer)
        {
            return;
        }

        //Debug.Log("Searching for music " + name);
        FindSound(name);

        if (currS.loop)
        {
            currSound = currS;
            //Debug.Log("Playing music " + name);
        }

        if (!currS.source.isPlaying || name != Constants.SoundEffects.grind)
        {
            currS.source.Play();
        }
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
