using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Image airBar;
    [SerializeField] private Image deathPanel;
    public Image DeathPanel { get { return deathPanel; } } 
    [SerializeField] private Text ringsText;
    [SerializeField] private Text maxRingsText;
    [SerializeField] private Text levelText;
    private float displayDelay = 0.05f;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform airBarParent = transform.GetChild(0);
        UpdateAirBar(100);
    }

    private void Update()
    {
        timer += Time.deltaTime;
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

    public void UpdateAirBar(float air)
    {
        airBar.fillAmount = air / 300;
    }

    public void UpdateSpeedText(float speed)
    {
        if (timer >= displayDelay)
        {
            int displaySpeed = Mathf.Abs(Mathf.RoundToInt(speed * 4));
            speedText.text = displaySpeed.ToString();
            timer = 0;
        }        
    }
}
