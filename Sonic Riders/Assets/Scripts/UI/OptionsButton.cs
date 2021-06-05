using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] private string[] options;
    [SerializeField] private List<int> optionEnum = new List<int>();

    private bool selected = false;
    private float prevHorInput;
    [SerializeField] private int optionIndex = 0;
    private bool selectingOption = false;

    private enum optionType { QUALITY }
    private optionType typeOption;

    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private Transform pointers;

    private void Start()
    {
        switch (typeOption)
        {
            case optionType.QUALITY:
                optionIndex = optionEnum.IndexOf(PlayerPrefs.GetInt("Quality", 5));

                UpdateText();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            float horInput = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(horInput) != Mathf.Abs(prevHorInput) && Mathf.Abs(horInput) > 0.3f)
            {
                SelectOption(horInput);
            }

            prevHorInput = horInput;
        }
    }

    private void SelectOption(float input)
    {
        if (selectingOption)
        {
            return;
        }

        selectingOption = true;

        if (input > 0 && optionIndex < options.Length - 1)
        {
            optionIndex++;            
        }
        else if (input < 0 && optionIndex > 0)
        {
            optionIndex--;
        }

        UpdateText();

        switch (typeOption)
        {
            case optionType.QUALITY:
                ChangeQuality();
                break;
        }

        selectingOption = false;
    }

    private void UpdateText()
    {
        optionText.text = options[optionIndex];

        if (optionIndex == options.Length - 1)
        {
            pointers.GetChild(1).gameObject.SetActive(false);
        }
        else if (optionIndex == 0)
        {
            pointers.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            pointers.GetChild(0).gameObject.SetActive(true);
            pointers.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(optionEnum[optionIndex]);
        PlayerPrefs.SetInt("Quality", QualitySettings.GetQualityLevel());
    }

    public void Selected(bool select)
    {
        selected = select;
    }
}
