using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlace : MonoBehaviour
{
    private HUD hud;
    [SerializeField] private Image numberImage;
    [SerializeField] private Transform afterNumber;
    [SerializeField] private GameObject glow;
    private bool updatePlacingBoard = false;
    private int placeOnBoard = 0;

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInParent<HUD>();
        glow = transform.GetChild(0).gameObject;
    }

    public void ChangeImageReferences(Image imageNumber, Transform numberAfter, GameObject firstGlow, int placing)
    {
        updatePlacingBoard = true;
        numberImage = imageNumber;
        afterNumber = numberAfter;
        glow = firstGlow;
        placeOnBoard = placing;
    }

    public void UpdatePlacing()
    {
        int place = hud.Place;

        if (updatePlacingBoard)
        {
            place = placeOnBoard;
        }

        if (place == 0)
        {
            glow.SetActive(true);
        }
        else
        {
            glow.SetActive(false);
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
