using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
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
    [SerializeField] private Animator placingAnim;
    private float displayDelay = 0.05f;
    private float timer = 0;

    [SerializeField] private Sprite[] airSprites;

    public RaceManager raceManager { get; set; }
    public int Place { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform airBarParent = transform.GetChild(0);
        UpdateAirBar(200, 200);
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void GiveRaceManager(RaceManager aRaceManager)
    {
        raceManager = aRaceManager;
    }

    public void ChangePlacing()
    {
        placingAnim.Play("ChangePlace");
    }

    public void UpdateRings(int rings, int maxRings)
    {
        ringsText.text = rings.ToString("000") + "/";
        maxRingsText.text = maxRings.ToString("000");
    }

    public void UpdateLevel(int level)
    {
        levelText.text = (level + 1).ToString();
    }

    public void UpdateAirBar(float air, float maxAir)
    {
        airBar.fillAmount = air / 300;
        maxAirBar.fillAmount = maxAir / 300;

        if (airBar.fillAmount < 0.5f)
        {
            airBar.sprite = airSprites[1];

            if (airBar.fillAmount < 0.25f)
            {
                airBar.color = Color.red;
                underAir.color = Color.red;
            }
            else
            {
                airBar.color = Color.white;
                underAir.color = new Color32(255, 145, 0, 255);
            }
        }
        else
        {
            airBar.sprite = airSprites[0];
            airBar.color = Color.white;
            underAir.color = new Color32(0, 229, 247, 255);
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
        lapText.text = lap.ToString("00") + "/";
        maxLapText.text = maxLap.ToString("00");
    }
}
