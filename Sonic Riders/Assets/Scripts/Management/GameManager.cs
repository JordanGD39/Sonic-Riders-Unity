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
    
    public void GetCams()
    {
        for (int i = 0; i < playersLeft.Count; i++)
        {
            cams.Add(playersLeft[i].GetComponentInChildren<Camera>());
        }

        Transform canvasHolder = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform;

        for (int i = 0; i < cams.Count; i++)
        {
            canvasHolder.GetChild(i).GetComponent<Canvas>().worldCamera = cams[i];
            cams[i].GetComponent<AudioListener>().enabled = i == 0;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<PlayerControls>().enabled = true;
        }
    }

}
