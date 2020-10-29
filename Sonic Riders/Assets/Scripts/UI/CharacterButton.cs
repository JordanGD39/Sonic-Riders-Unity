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
            image.color = playerSelect.NotJoinedPanel.transform.GetChild(0).GetComponent<Outline>().effectColor;

            if (playerSelect.NotJoinedPanel.activeSelf)
            {
                playerSelect.NotJoinedPanel.SetActive(false);
                playerSelect.JoinedPanel.SetActive(true);
            }

            playerSelect.CharacterImage.sprite = sprite.sprite;

            for (int i = 0; i < playerSelect.TypeParent.childCount; i++)
            {
                playerSelect.TypeParent.GetChild(i).gameObject.SetActive((int)someStats.CharType == i);
            }
        }

        Transform statsParent = playerSelect.StatsParent;

        for (int i = 0; i < statsParent.childCount; i++)
        {
            DisplayStats(i, statsParent.GetChild(i), someStats);
        }
    }

    private void DisplayStats(int index, Transform statTransform, CharacterStats someStats)
    {
        float stat = 0.5f;
        float minValue = 0;
        switch (index)
        {
            case 0:
                minValue = 7;
                stat = (someStats.GetCurrentDash() - minValue) / 20;
                break;
            case 1:
                minValue = 25;
                stat = (someStats.GetCurrentLimit() - minValue) / 60;
                break;
            case 2:
                minValue = 15;
                stat = (someStats.GetCurrentPower() - minValue) / 46.75f;
                break;
            case 3:
                minValue = 40;
                stat = (someStats.GetCurrentCornering() - minValue) / 160;
                break;
            case 4:
                stat = someStats.GetCurrentAirLoss() / 5;
                break;
            case 5:
                minValue = 30;
                stat = (someStats.RunSpeed - minValue) / 20;
                break;
        }

        statTransform.GetChild(0).GetComponent<Image>().fillAmount = stat;
    }

    public void Pressed(int pressedIndex)
    {
        Transform selectTransform = GameManager.instance.transform.transform.GetChild(0);

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
            ShowSelectedCharacter(index, false, null);
        }
        else
        {
            image.color = new Color32(166, 165, 166, 255);
        }

        alreadyDeselecting = false;
    }
}
