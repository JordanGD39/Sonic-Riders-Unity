using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlace : MonoBehaviour
{
    private HUD hud;
    [SerializeField] private Image numberImage;
    [SerializeField] private Transform afterNumber;

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInParent<HUD>();
    }

    public void UpdatePlacing()
    {
        int place = hud.Place;

        if (place == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        numberImage.sprite = hud.raceManager.NumberSprites[place];

        if (place > 3)
        {
            place = 3;
        }

        for (int i = 0; i < afterNumber.childCount; i++)
        {
            afterNumber.GetChild(i).gameObject.SetActive(false);
        }

        afterNumber.GetChild(place).gameObject.SetActive(true);
    }
}
