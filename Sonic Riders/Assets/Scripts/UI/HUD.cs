using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : SurvivalFunctionsUI
{
    [SerializeField] private RectTransform airBarTransform;
    [SerializeField] private RectTransform ringTransform;
    [SerializeField] private RectTransform distanceTransform;
    [SerializeField] private Text speedText;
    [SerializeField] private Image airBar;
    [SerializeField] private Image underAir;
    [SerializeField] private Image maxAirBar;
    [SerializeField] private Image deathPanel;
    public Image DeathPanel { get { return deathPanel; } } 
    [SerializeField] private Text ringsText;
    [SerializeField] private Text maxRingsText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text lapText;
    [SerializeField] private Text maxLapText;
    [SerializeField] private GameObject distanceRadar;
    [SerializeField] private RectTransform airBarParent;
    [SerializeField] private RectTransform textSpeed;
    public GameObject DistanceRadar { get { return distanceRadar; } }
    [SerializeField] private GameObject placingUI;
    public GameObject PlacingUI { get { return placingUI; } }
    
    [SerializeField] private Animator placingAnim;
    private float displayDelay = 0.05f;
    private float timer = 0;
    public float LevelThreeMaxAir { get; set; } = 200;

    public RaceManager raceManager { get; set; }
    public int Place { get; set; } = 0;

    [SerializeField] private Sprite[] itemSprites;
    public Sprite[] ItemSprites { get { return itemSprites; } }
    [SerializeField] private ItemUI[] itemUis;

    public bool AlreadyOn { get; set; } = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        distanceRadar.SetActive(false);

        ReadyToChange = true;

        if (!AlreadyOn)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void TwoPlayersHud()
    {
        airBarTransform.anchoredPosition = new Vector2(0, airBarTransform.anchoredPosition.y);
        ringTransform.anchoredPosition = new Vector2(0, airBarTransform.anchoredPosition.y);
        distanceTransform.anchoredPosition = new Vector2(0, airBarTransform.anchoredPosition.y);
    }

    public void GiveRaceManager(RaceManager aRaceManager)
    {
        raceManager = aRaceManager;
    }

    public void ReverseAirBar()
    {
        airBarParent.localScale = new Vector3(-1, 1, 1);
        speedText.GetComponent<RectTransform>().localScale = new Vector3(-0.4f, 0.4f, 1);
        textSpeed.localScale = new Vector3(-1, 1, 1);
    }

    public void ChangePlacing()
    {
        placingAnim.SetTrigger("UpdatePlacing");
    }

    public void UpdateRings(int rings, int maxRings, bool ringsAsAir)
    {
        string slash = "";

        if (rings < 60 && !ringsAsAir)
        {
            slash = "/";
            maxRingsText.text = maxRings.ToString("000");
        }
        else
        {
            maxRingsText.text = "";
        }

        ringsText.text = rings.ToString("000") + slash;   
    }

    public void UpdateLevel(int level)
    {
        levelText.text = (level + 1).ToString();
    }

    public void UpdateAirBar(float air, float maxAir)
    {
        airBar.fillAmount = air / LevelThreeMaxAir;
        maxAirBar.fillAmount = maxAir / LevelThreeMaxAir;

        if (airBar.fillAmount < 0.5f)
        {
            if (airBar.fillAmount < 0.25f)
            {
                float fill = airBar.fillAmount;

                Color finalColor = new Color(1, fill + 0.75f, 0.25f);

                airBar.color = finalColor;
                underAir.color = new Color(1, fill + 0.75f, 0.25f, fill / 0.25f);
            }
            else
            {
                float fill = airBar.fillAmount;

                Color finalColor = new Color(1, 1, fill);

                airBar.color = finalColor;
                underAir.color = finalColor;
            }
        }
        else
        {
            float fill = airBar.fillAmount;
            float red = (fill - 0.5f) / 0.5f;
            red -= 1;

            Color finalColor = new Color(-red, 1, 0.5f);

            airBar.color = finalColor;
            underAir.color = finalColor;
        }
    }

    public void UpdateSpeedText(float speed)
    {
        if (timer >= displayDelay)
        {
            int displaySpeed = Mathf.Abs(Mathf.RoundToInt(speed * 3));
            speedText.text = displaySpeed.ToString();
            timer = 0;
        }        
    }

    public void UpdateLap(int lap, int maxLap)
    {
        if (lap > maxLap)
        {
            lap = maxLap;
        }

        lapText.text = lap.ToString("00") + "/";
        maxLapText.text = maxLap.ToString("00");
    }    

    public void ShowItem(int index, int amount)
    {
        itemUis[0].gameObject.SetActive(true);
        itemUis[0].UpdateText(index, amount);
    }
}
