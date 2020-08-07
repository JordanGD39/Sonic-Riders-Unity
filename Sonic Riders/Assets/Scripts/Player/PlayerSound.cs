using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] voiceClips;
    [SerializeField] private AudioClip[] playerSfx;
    private AudioSource source;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource grindSource;
    public enum voiceSounds {NONE, OPENING, JUMPRAMP, PERFECTJUMP, RAMPFAIL, JUMPSUCCES, CRASH, LOSE, WIN}
    public enum sounds {NONE, BOOST, JUMP, LAND, GRIND}

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(voiceSounds voiceClipToPlay, sounds soundToPlay)
    {
        if (voiceClipToPlay != voiceSounds.NONE)
        {
            source.PlayOneShot(voiceClips[(int)voiceClipToPlay - 1]);
        }
        else if (soundToPlay != sounds.NONE)
        {
            if (soundToPlay != sounds.GRIND)
            {
                sfxSource.PlayOneShot(playerSfx[(int)soundToPlay - 1]);
            }
            else
            {
                if (!grindSource.isPlaying)
                {
                    grindSource.Play();
                }                
            }
        }
    }

    public void StopPlayingGrind()
    {
        grindSource.Stop();
    }
}
