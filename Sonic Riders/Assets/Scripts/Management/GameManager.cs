using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject campPref;
    [SerializeField] private GameObject playerInputPref;
    [SerializeField] private Transform playersParent;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingScreenImage;
    public GameObject LoadingScreen { get { return loadingScreen; } }
    [SerializeField] private Animator loadingScreenAnim;
    private PlayerConfigManager playerConfigManager;
    [SerializeField] private Image trail;

    public AudioManager GetAudioManager { get { return audioManager; } }

    [SerializeField] private List<GameObject> playersLeft = new List<GameObject>();
    public List<GameObject> PlayersLeft { get { return playersLeft; } }

    [SerializeField] private List<string> playerNames = new List<string>();
    public List<string> PlayersNames { get { return playerNames; } }

    private List<Camera> cams = new List<Camera>();
    public List<Camera> Cams { get { return cams; } }

    [SerializeField] private float gravityMultiplier = 1;
    public float GravitityMultiplier { get { return gravityMultiplier; } }
    [SerializeField] private bool testAir = true;
    public bool TestAir { get { return testAir; } }

    private PlayerInputManager playerInputManager;
    private string currScene = "";
    public string CurrScene { get { return currScene; } }

    public string TrackToLoad { get; set; } = "SampleScene 1";
    public enum gamemode {RACE, SURVIVAL, TUTORIAL};
    public gamemode GameMode = gamemode.RACE;

    private bool loadingScene = false;
    public bool LoadingScene { get { return loadingScene; } }

    [SerializeField] private bool fishPhobia = false;
    public bool FishPhobia { get { return fishPhobia; } }
    
    private bool fadeTheMusic = false;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInputManager = GetComponent<PlayerInputManager>();
        currScene = SceneManager.GetActiveScene().name;
        //Application.targetFrameRate = 5;
        Physics.gravity *= gravityMultiplier;
        loadingScreen.SetActive(false);
        loadingScreenAnim = loadingScreen.GetComponent<Animator>();
        playerConfigManager = GetComponent<PlayerConfigManager>();

        if (playersLeft.Count > 0)
        {
            for (int i = 0; i < playersLeft.Count; i++)
            {
                playersLeft[i].SetActive(true);
            }

            GetCams();
        }
    }

    public void LoadScene(string sceneName, bool fadeMusic)
    {
        if (loadingScene)
        {
            return;
        }

        Debug.Log("Loading new scene");

        loadingScene = true;
        currScene = sceneName;
        fadeTheMusic = fadeMusic;
        loadingScreen.SetActive(true);
        audioManager.FadeOutRate = 0.01f;

        if (fadeMusic)
        {
            audioManager.StartFadeVoid(false);
        }

        //StopCoroutine("LoadSceneAsync");
        StartCoroutine("LoadSceneAsync");
    }

    private IEnumerator LoadSceneAsync()
    {
        while (loadingScreenImage.color.a != 1)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation ao = SceneManager.LoadSceneAsync(currScene);

        float prevProgress = 0;
        //ao.allowSceneActivation = false;
        //loadingScreenAnim.Play("SonicFadeIn");

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);

            float diff = progress - prevProgress;
            //Debug.Log(diff);

            loadingScreenAnim.SetFloat("Speed", diff);
            trail.fillAmount = progress;

            prevProgress = progress;
            
            yield return null;
        }

        if (fadeTheMusic)
            audioManager.StartFadeVoid(true);

        Time.timeScale = 1;
        yield return new WaitForSeconds(1);

        loadingScreenAnim.Play("LoadFadeOut");

        yield return new WaitForSeconds(1);        

        trail.fillAmount = 0;
        loadingScreen.SetActive(false);        
        loadingScene = false;

        if (currScene == "CharacterSelect")
        {
            if (GameMode == gamemode.TUTORIAL)
            {
                playerConfigManager.MaxPlayers = 1;
            }
            else
            {
                playerConfigManager.MaxPlayers = 4;
            }

            playerInputManager.EnableJoining();
            playerConfigManager.FindCanvas();
            playerConfigManager.ClearEventSystems();

            for (int i = 0; i < playersParent.childCount; i++)
            {
                playerConfigManager.CreateEventSystem(playersParent.GetChild(i).GetComponent<PlayerInput>());
                playersParent.GetChild(i).GetComponent<CharacterSelectInput>().StartFunctions();
                playersParent.GetChild(i).GetComponent<PlayerControls>().ControlsDisable();
            }
        }
    }

    public void GetCams()
    {
        cams.Clear();

        for (int i = 0; i < playersLeft.Count; i++)
        {
            cams.Add(playersLeft[i].GetComponentInChildren<Camera>());
        }       

        Transform canvasHolder = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).transform;

        for (int i = 0; i < cams.Count; i++)
        {
            canvasHolder.GetChild(i).GetComponent<Canvas>().worldCamera = cams[i].transform.GetChild(0).GetComponent<Camera>();
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

        for (int i = 0; i < playersParent.childCount; i++)
        {
            PlayerControls controls = playersParent.GetChild(i).GetComponent<PlayerControls>();
            controls.FindPlayer();            
        }
    }

}
