using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text speedText;
    private Image airBar;

    // Start is called before the first frame update
    void Start()
    {
        Transform airBarParent = transform.GetChild(0);
        speedText = airBarParent.GetChild(3).GetComponent<Text>();
        airBar = airBarParent.GetChild(1).GetComponent<Image>();
        UpdateAirBar(100);
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
