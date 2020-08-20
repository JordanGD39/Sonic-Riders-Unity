using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;

    public bool loop = false;

    [Range(0f, 1f)]
    public float spatialBlend = 1;

    public float minRange = 1;
    public float maxRange = 100;

    [HideInInspector]
    public AudioSource source;
}
