using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] voiceClips;
    private AudioSource source;
    public enum sounds { OPENING, JUMPRAMP, PERFECTJUMP, RAMPFAIL, JUMPSUCCES, CRASH, LOSE, WIN}

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(sounds soundToPlay)
    {
        if (!source.isPlaying)
        {
            source.PlayOneShot(voiceClips[(int)soundToPlay]);
        }        
    }
}
