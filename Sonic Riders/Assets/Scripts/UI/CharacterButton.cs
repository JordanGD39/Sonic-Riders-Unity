using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterButton : MonoBehaviour
{
    private EventSystemHolder holder;

    [SerializeField] private Image sprite;
    private Image image;

    private Transform playerSelectParent;
    [SerializeField] private GameObject disabled;
    public GameObject DisabledImage { get { return disabled; } }
    [SerializeField] private GameObject characterPrefab;
    private GameObject CharacterPrefab { get { return characterPrefab; } }
    //private BoardStats boardStats;
    private CharacterStats stats;
    
    private bool alreadyDeselecting = false;

    // Start is called before the first frame update
    void Start()
    {
        holder = GetComponentInParent<EventSystemHolder>();
        playerSelectParent = holder.GetComponentInChildren<GridLayoutGroup>().transform;
        stats = characterPrefab.GetComponent<CharacterStats>();
        image = GetComponent<Button>().image;
    }

    public void ShowSelectedCharacter(int index, bool selectingBoard, CharacterStats updatedStats)
    {
        CharacterStats someStats = stats;

        if (updatedStats != null)
        {
            someStats = updatedStats;
        }

        PlayerSelectReferences playerSelect = playerSelectParent.GetChild(index).GetComponent<PlayerSelectReferences>();

        if (!selectingBoard)
        {
            image.color = playerSelect.NotJoinedPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().outlineColor;

            if (playerSelect.NotJoinedPanel.activeSelf)
            {
                playerSelect.NotJoinedPanel.SetActive(false);
                playerSelect.JoinedPanel.SetActive(true);
            }

            playerSelect.CharacterImage.sprite = sprite.sprite;

            playerSelect.FullName.text = disabled.activeSelf ? "" : someStats.FullName.ToUpper();

            for (int i = 0; i < playerSelect.TypeParent.childCount; i++)
            {
                playerSelect.TypeParent.GetChild(i).gameObject.SetActive((int)someStats.CharType == i && !disabled.activeSelf);
            }
        }

        Transform statsParent = playerSelect.StatsParent;

        for (int i = 0; i < statsParent.childCount; i++)
        {
            DisplayStats(i, statsParent.GetChild(i), someStats);
        }
    }

    public void RemovePlayer(int index)
    {
        image.color = new Color32(166, 165, 166, 255);

        holder.MultiplayerEventSystems.RemoveAt(index);
    }

    private void DisplayStats(int index, Transform statTransform, CharacterStats someStats)
    {
        float stat = 0.5f;
        float minValue = 0;
        switch (index)
        {
            case 0:
                minValue = 7;
                stat = (someStats.GetCurrentDash() - minValue) / (22 - minValue);
                break;
            case 1:
                minValue = 40;
                stat = (someStats.GetCurrentLimit() - minValue) / (80 - minValue);
                break;
            case 2:
                minValue = 15;
                stat = (someStats.GetCurrentPower() - minValue) / (46.75f - minValue);
                break;
            case 3:
                minValue = 40;
                stat = (someStats.GetCurrentCornering() - minValue) / (100 - minValue);
                break;
            case 4:
                if (someStats.BoardStats.RingsAsAir)
                {
                    statTransform.GetComponent<TextMeshProUGUI>().text = "Ring loss";
                }
                else
                {
                    statTransform.GetComponent<TextMeshProUGUI>().text = "Air loss";
                }
                stat = someStats.GetCurrentAirLoss() / 5;
                break;
            case 5:
                minValue = 30;
                stat = (someStats.RunSpeed - minValue) / (50 - minValue);
                break;
        }

        statTransform.GetChild(0).GetComponent<Image>().fillAmount = stat;
    }

    /*public void Pressed(int pressedIndex)
    {
        Transform selectTransform = GameManager.instance.transform.transform.GetChild(0);

        CharacterSelectInput selectInput = selectTransform.GetChild(pressedIndex).GetComponent<CharacterSelectInput>();

        if (selectInput.CanSelect && !disabled.activeSelf)
        {
            holder.MultiplayerEventSystems[pressedIndex].SetSelectedGameObject(null);
            selectInput.ConfirmCharacter(characterPrefab);
        }
    }*/    

    public GameObject CheckCharacter()
    {
        if (disabled.activeSelf)
        {
            return null;
        }
        else
        {
            return characterPrefab;
        }
    }

    public void Deselected()
    {
        if (!alreadyDeselecting)
        {            
            alreadyDeselecting = true;
            StartCoroutine("DelaySelect");        
        }
    }

    private IEnumerator DelaySelect()
    {
        yield return null;

        int index = -1;

        Transform selectTransform = GameManager.instance.transform.transform.GetChild(0);

        for (int i = 0; i < selectTransform.childCount; i++)
        {
            CharacterSelectInput selectInput = selectTransform.GetChild(i).GetComponent<CharacterSelectInput>();

            if (selectInput.EventSystem.currentSelectedGameObject == gameObject)
            {
                Debug.Log("Found eventSystem selecting me! " + selectInput.PlayerIndex);
                index = selectInput.PlayerIndex;
            }
        }

        if (index > -1)
        {
            ShowSelectedCharacter(index, false, null);
        }
        else
        {
            image.color = new Color32(166, 165, 166, 255);
        }

        alreadyDeselecting = false;
    }
}
