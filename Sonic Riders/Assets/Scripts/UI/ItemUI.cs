using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Text itemText;
    [SerializeField] private Image itemImage;

    private HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInParent<HUD>();
        gameObject.SetActive(false);
    }

    public void UpdateText(int itemIndex, int amount)
    {
        itemImage.sprite = hud.ItemSprites[itemIndex];

        if (amount != 0)
        {
            itemText.text = amount.ToString();
        }
        else
        {
            itemText.text = "";
        }
    }

    public void SetUnactive()
    {
        gameObject.SetActive(false);
    }
}
