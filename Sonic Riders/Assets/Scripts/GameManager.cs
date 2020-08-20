using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private AudioManager audioManager;
    public AudioManager GetAudioManager { get { return audioManager; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/
}
