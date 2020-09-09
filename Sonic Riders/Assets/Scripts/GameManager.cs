using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private AudioManager audioManager;
    public AudioManager GetAudioManager { get { return audioManager; } }

    [SerializeField] private List<GameObject> playersLeft = new List<GameObject>();
    public List<GameObject> PlayersLeft { get { return playersLeft; } }
    private List<Camera> cams = new List<Camera>();
    public List<Camera> Cams { get { return cams; } }

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
        for (int i = 0; i < playersLeft.Count; i++)
        {
            cams.Add(playersLeft[i].GetComponentInChildren<Camera>());
        }
        //audioManager = GetComponent<AudioManager>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/
}
