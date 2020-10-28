using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    private EventSystemHolder holder;

    [SerializeField] private Image sprite;
    private Image image;

    private Transform playerSelectParent;
    [SerializeField] private GameObject disabled;
    public GameObject DisabledImage { get { return disabled; } }
    [SerializeField] private GameObject characterPrefab;
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

    public void Selected()
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < holder.MultiplayerEventSystems.Count; i++)
        {
            if (holder.MultiplayerEventSystems[i].currentSelectedGameObject == gameObject)
            {
                indexes.Add(i);
            }
        }

        for (int i = 0; i < indexes.Count; i++)
        {
            ShowSelectedCharacter(indexes[i]);
        }
    }

    private void ShowSelectedCharacter(int index)
    {
        Transform playerSelect = playerSelectParent.GetChild(index);

        image.color = playerSelect.GetChild(0).GetChild(0).GetComponent<Outline>().effectColor;

        if (playerSelect.GetChild(0).gameObject.activeSelf)
        {
            playerSelect.GetChild(0).gameObject.SetActive(false);
            playerSelect.GetChild(1).gameObject.SetActive(true);
        }

        playerSelect.GetChild(1).GetChild(1).GetComponent<Image>().sprite = sprite.sprite;

        Transform typeParent = playerSelect.GetChild(1).GetChild(0);

        for (int i = 0; i < typeParent.childCount; i++)
        {
            Debug.Log(typeParent.GetChild(i).gameObject);
            typeParent.GetChild(i).gameObject.SetActive((int)stats.CharType == i);
        }

        Transform statsParent = playerSelect.GetChild(1).GetChild(2);

        for (int i = 0; i < statsParent.childCount; i++)
        {
            DisplayStats(i, statsParent.GetChild(i));
        }
    }

    private void DisplayStats(int index, Transform statTransform)
    {
        float stat = 0.5f;
        float minValue = 0;
        switch (index)
        {
            case 0:
                minValue = 7;
                stat = (stats.GetCurrentDash() - minValue) / 20;
                break;
            case 1:
                minValue = 25;
                stat = (stats.GetCurrentLimit() - minValue) / 60;
                break;
            case 2:
                minValue = 15;
                stat = (stats.GetCurrentPower() - minValue) / 46.75f;
                break;
            case 3:
                minValue = 40;
                stat = (stats.GetCurrentCornering() - minValue) / 160;
                break;
            case 4:
                stat = stats.GetCurrentAirLoss() / 5;
                break;
            case 5:
                minValue = 30;
                stat = (stats.RunSpeed - minValue) / 20;
                break;
        }

        statTransform.GetChild(0).GetComponent<Image>().fillAmount = stat;
    }

    public void Pressed()
    {
        Transform selectTransform = GameManager.instance.transform.transform.GetChild(0);

        int pressedIndex = 0;

        for (int i = 0; i < holder.MultiplayerEventSystems.Count; i++)
        {
            if (holder.MultiplayerEventSystems[i].currentSelectedGameObject == gameObject && selectTransform.GetChild(i).GetComponent<CharacterSelectInput>().PressedButton)
            {
                Debug.Log(pressedIndex);
                pressedIndex = i;
            }
        }

        CharacterSelectInput selectInput = selectTransform.GetChild(pressedIndex).GetComponent<CharacterSelectInput>();

        if (selectInput.CanSelect && !disabled.activeSelf)
        {
            holder.MultiplayerEventSystems[pressedIndex].SetSelectedGameObject(null);
            selectInput.ConfirmCharacter(characterPrefab);
        }
    }

    public void Deselected()
    {
        if (!alreadyDeselecting)
        {
            image.color = new Color32(166, 165, 166, 255);
            alreadyDeselecting = true;
            StartCoroutine("DelaySelect");        
        }
    }

    private IEnumerator DelaySelect()
    {
        yield return null;

        int index = -1;

        for (int i = 0; i < holder.MultiplayerEventSystems.Count; i++)
        {
            if (holder.MultiplayerEventSystems[i].currentSelectedGameObject == gameObject)
            {
                Debug.Log("Found eventSystem selecting me! " + i);
                index = i;
            }
        }

        if (index > -1)
        {
            holder.MultiplayerEventSystems[index].SetSelectedGameObject(null);
            holder.MultiplayerEventSystems[index].SetSelectedGameObject(gameObject);
        }

        alreadyDeselecting = false;
    }
}
