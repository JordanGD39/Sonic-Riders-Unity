using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : SurvivalFunctionsUI
{
    [SerializeField] private RectTransform airBarTransform;
    [SerializeField] private RectTransform ringTransform;
    [SerializeField] private RectTransform distanceTransform;
    [SerializeField] private RectTransform placingTransform;
    [SerializeField] private TextMeshProUGUI speedText;
    //[SerializeField] private Image[] speedDigits;
    [SerializeField] private Image airBar;
    [SerializeField] private Image underAir;
    [SerializeField] private Image maxAirBar;
    [SerializeField] private Image deathPanel;
    public Image DeathPanel { get { return deathPanel; } } 
    [SerializeField] private TextMeshProUGUI ringsText;
    [SerializeField] private TextMeshProUGUI maxRingsText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI maxLapText;
    [SerializeField] private GameObject distanceRadar;
    [SerializeField] private GameObject cautionAir;
    [SerializeField] private GameObject cautionText;
    [SerializeField] private RectTransform airBarParent;
    [SerializeField] private RectTransform textSpeed;
    public GameObject DistanceRadar { get { return distanceRadar; } }
    [SerializeField] private GameObject placingUI;
    public GameObject PlacingUI { get { return placingUI; } }
    [SerializeField] private GameObject rankUI;
    [SerializeField] private Image rankImage;
    [SerializeField] private TextMeshProUGUI gainedAirRank;    
    [SerializeField] private Sprite[] rankSprites;
    [SerializeField] private Sprite[] commentTimeSprites;
    
    [SerializeField] private TextMeshProUGUI startingTimeText;
    [SerializeField] private TextMeshProUGUI extraSpeedStartText;
    [SerializeField] private Image commentTimeImage;

    [SerializeField] private GameObject startImage;
    [SerializeField] private GameObject lapImage;
    [SerializeField] private GameObject finalLapImage;
    [SerializeField] private Transform lapDigits;

    [SerializeField] private Animator placingAnim;
    private float displayDelay = 0.05f;
    private float timer = 0;
    public float LevelThreeMaxAir { get; set; } = 200;

    public RaceManager raceManager { get; set; }
    public int Place { get; set; } = 0;

    [SerializeField] private Sprite[] itemSprites;
    public Sprite[] ItemSprites { get { return itemSprites; } }
    [SerializeField] private ItemUI[] itemUis;

    [SerializeField] private CharToImageUI charToImageUI;

    public bool AlreadyOn { get; set; } = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        commentTimeImage.gameObject.SetActive(false);
        finalLapImage.SetActive(false);
        lapImage.transform.parent.gameObject.SetActive(false);
        startImage.SetActive(false);
        startingTimeText.transform.parent.gameObject.SetActive(false);
        distanceRadar.SetActive(false);
        rankUI.SetActive(false);
        cautionAir.SetActive(false);

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

    public void ShowStart()
    {
        startImage.SetActive(true);
    }

    public void ShowStartingTime(float time, float extraSpeed)
    {
        string plusString = time > 0 ? "+" : "-";

        Debug.Log("Given Time: " + time);

        float seconds = time > 0 ? Mathf.Floor(time % 60) : Mathf.Ceil(time % 60);
         
        string secondsText = Mathf.Abs(seconds).ToString("00");
        float centiseconds = Mathf.Abs(time * 100);
        centiseconds = centiseconds % 100;

        int commentIndex = 0;

        if (time < 0)
        {
            if (centiseconds >= 0 && centiseconds <= 20)
            {
                commentIndex = 3;
            }
            else if (centiseconds > 20 && centiseconds <= 50)
            {
                commentIndex = 2;
            }
            else if (centiseconds > 50 && centiseconds < 100)
            {
                commentIndex = 1;
            }                      
        }

        commentTimeImage.sprite = commentTimeSprites[commentIndex];
        commentTimeImage.gameObject.SetActive(true);

        startingTimeText.text = plusString + secondsText + "''" + centiseconds.ToString("00");
        extraSpeedStartText.text = "+" + Mathf.Round(extraSpeed * 3);
        startingTimeText.transform.parent.gameObject.SetActive(true);
    }

    public void ToggleCaution(bool active, bool showText)
    {
        cautionText.SetActive(showText);
        cautionAir.SetActive(active);
    }

    public void ShowRank(int index, int airGained)
    {
        rankImage.sprite = rankSprites[index];
        gainedAirRank.text = "+" + airGained.ToString("000");

        rankUI.SetActive(false);
        rankUI.SetActive(true);
    }

    public void TwoPlayersHud(int playerNum)
    {
        airBarTransform.anchoredPosition = new Vector2(0, airBarTransform.anchoredPosition.y);
        if (playerNum == 0)
        {
            ringTransform.anchoredPosition = new Vector2(-263, ringTransform.anchoredPosition.y);
        }
        else
        {
            ringTransform.anchoredPosition = new Vector2(263, ringTransform.anchoredPosition.y);
        }
        distanceTransform.anchoredPosition = new Vector2(0, distanceTransform.anchoredPosition.y);
        placingTransform.anchoredPosition = new Vector2(-223, placingTransform.anchoredPosition.y);
    }

    public void UndoTwoPlayersHud(int playerNum)
    {
        airBarTransform.anchoredPosition = new Vector2(-400, airBarTransform.anchoredPosition.y);
        if (playerNum == 0)
        {
            ringTransform.anchoredPosition = new Vector2(163, ringTransform.anchoredPosition.y);
        }
        else
        {
            ringTransform.anchoredPosition = new Vector2(-140, ringTransform.anchoredPosition.y);
        }
        distanceTransform.anchoredPosition = new Vector2(134, distanceTransform.anchoredPosition.y);
        placingTransform.anchoredPosition = new Vector2(128, placingTransform.anchoredPosition.y);
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
        maxRingsText.text = !ringsAsAir ? maxRings.ToString("000") : "100";     
        ringsText.text = rings.ToString("000") + "/";   
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

            speedText.text = displaySpeed.ToString("000");

            timer = 0;
        }        
    }

    public void UpdateLap(int lap, int maxLap)
    {
        bool endLapReached = false;

        if (lap > maxLap)
        {
            endLapReached = true;
            lap = maxLap;
        }

        lapText.text = lap.ToString("00");
        maxLapText.text = maxLap.ToString("00");

        if (lap == 1 || endLapReached)
        {
            return;
        }

        if (lap < maxLap)
        {
            Sprite[] numberSprites = charToImageUI.ConvertCharsToSprite(lap, "00", CharToImageUI.numberType.LAP);

            lapDigits.GetChild(0).GetComponent<Image>().sprite = numberSprites[0];
            lapDigits.GetChild(1).GetComponent<Image>().sprite = numberSprites[1];
        }
        else
        {
            lapImage.SetActive(false);
            finalLapImage.SetActive(true);
            lapDigits.gameObject.SetActive(false);
        }

        lapImage.transform.parent.gameObject.SetActive(false);
        lapImage.transform.parent.gameObject.SetActive(true);
    }    

    public void ShowItem(int index, int amount)
    {
        itemUis[0].gameObject.SetActive(true);
        itemUis[0].UpdateText(index, amount);
    }
}
