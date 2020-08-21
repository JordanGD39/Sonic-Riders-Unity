using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Image airBar;
    [SerializeField] private Text ringsText;
    [SerializeField] private Text maxRingsText;
    [SerializeField] private Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        Transform airBarParent = transform.GetChild(0);
        UpdateAirBar(100);
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
        int displaySpeed = Mathf.Abs(Mathf.RoundToInt(speed * 4));
        speedText.text = displaySpeed.ToString();
    }
}
