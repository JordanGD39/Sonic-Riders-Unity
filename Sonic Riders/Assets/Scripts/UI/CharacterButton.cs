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

    private int eventIndex = 0;
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
        for (int i = 0; i < holder.MultiplayerEventSystems.Count; i++)
        {
            if (holder.MultiplayerEventSystems[i].currentSelectedGameObject == gameObject)
            {
                eventIndex = i;
            }
        }


        Transform playerSelect = playerSelectParent.GetChild(eventIndex);

        image.color = playerSelect.GetChild(0).GetChild(0).GetComponent<Outline>().effectColor;
            
        if (playerSelect.GetChild(0).gameObject.activeSelf)
        {
            playerSelect.GetChild(0).gameObject.SetActive(false);
            playerSelect.GetChild(1).gameObject.SetActive(true);
        }

        playerSelect.GetChild(1).GetComponentInChildren<Image>().sprite = sprite.sprite;

        Transform statsParent = playerSelect.GetChild(1).GetComponentInChildren<Text>().transform.parent;

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
        CharacterSelectInput selectInput = GameManager.instance.transform.transform.GetChild(0).GetChild(eventIndex).GetComponent<CharacterSelectInput>();

        if (!disabled.activeSelf && selectInput.CanSelect)
        {
            disabled.SetActive(true);
            holder.MultiplayerEventSystems[eventIndex].SetSelectedGameObject(null);
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
