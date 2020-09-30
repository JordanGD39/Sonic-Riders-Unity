using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject campPref;
    public AudioManager GetAudioManager { get { return audioManager; } }

    [SerializeField] private List<GameObject> playersLeft = new List<GameObject>();
    public List<GameObject> PlayersLeft { get { return playersLeft; } }
    private List<Camera> cams = new List<Camera>();
    public List<Camera> Cams { get { return cams; } }

    [SerializeField] private float gravityMultiplier = 1;
    public float GravitityMultiplier { get { return gravityMultiplier; } }
    [SerializeField] private bool testAir = true;
    public bool TestAir { get { return testAir; } }

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

    private void Start()
    {
        Physics.gravity *= gravityMultiplier;

        if (SceneManager.GetActiveScene().buildIndex > 0 && playersLeft.Count > 0)
        {
            for (int i = 0; i < playersLeft.Count; i++)
            {
                playersLeft[i].SetActive(true);
            }

            GetCams();
        }
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
            cams[i].depth = -i;
            //cams[i].transform.parent.parent = null;
            //playersLeft[i].GetComponent<PlayerInput>().camera = cams[i];
        }

        if (cams.Count == 3)
        {
            GameObject cam = Instantiate(campPref);
            cams.Add(cam.GetComponent<Camera>());
        }

        if (GetComponent<TestHandleJoin>() != null)
        {
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<PlayerControls>().enabled = true;
        }
    }

}
