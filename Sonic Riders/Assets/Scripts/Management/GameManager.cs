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
    [SerializeField] private Animator loadingScreenAnim;
    private PlayerConfigManager playerConfigManager;
    [SerializeField] private Image trail;

    public AudioManager GetAudioManager { get { return audioManager; } }

    [SerializeField] private List<GameObject> playersLeft = new List<GameObject>();
    public List<GameObject> PlayersLeft { get { return playersLeft; } }
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
        currScene = sceneName;
        loadingScreen.SetActive(true);
        audioManager.FadeOutRate = 0.01f;

        if (fadeMusic)
        {
            audioManager.StartFadeVoid(false);
        }

        StartCoroutine(LoadSceneAsync(sceneName, fadeMusic));
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool fadeMusic)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

        float prevProgress = 0;
        bool sonicOnScreen = false;
        //ao.allowSceneActivation = false;
        //loadingScreenAnim.Play("SonicFadeIn");

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);

            float diff = progress - prevProgress;
            Debug.Log(diff);

            if (sonicOnScreen)
            {
                loadingScreenAnim.SetFloat("Speed", diff);
                trail.fillAmount = progress;
            }
            else if(diff < 0.016f && diff > 0)
            {
                loadingScreenAnim.Play("SonicFadeIn");
                sonicOnScreen = true;
            }

            prevProgress = progress;
            
            yield return null;
        }

        if (sonicOnScreen)
        {
            loadingScreenAnim.Play("LoadFadeOut");
        }
        else
        {
            loadingScreenAnim.Play("LoadFadeOutNoSonic");
        }

        if (fadeMusic)
            audioManager.StartFadeVoid(true);

        yield return new WaitForSeconds(1);

        loadingScreen.SetActive(false);

        if (sceneName == "CharacterSelect")
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

        for (int i = 0; i < playersParent.childCount; i++)
        {
            PlayerControls controls = playersParent.GetChild(i).GetComponent<PlayerControls>();
            controls.FindPlayer();            
        }
    }

}
