using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    private EventSystemHolder holder;

    [SerializeField] private Image sprite;
    [SerializeField] private int dash = 3;
    [SerializeField] private int limit = 4;
    [SerializeField] private int power = 3;
    [SerializeField] private int cornering = 2;

    private Transform playerSelectParent;
    [SerializeField] private GameObject disabled;
    [SerializeField] private GameObject characterPrefab;

    private int eventIndex = 0;
    private bool alreadyDeselecting = false;

    // Start is called before the first frame update
    void Start()
    {
        holder = GetComponentInParent<EventSystemHolder>();
        playerSelectParent = holder.GetComponentInChildren<GridLayoutGroup>().transform;
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

        if (playerSelect.GetChild(0).gameObject.activeSelf)
        {
            playerSelect.GetChild(0).gameObject.SetActive(false);
            playerSelect.GetChild(1).gameObject.SetActive(true);
        }

        playerSelect.GetChild(1).GetComponentInChildren<Image>().sprite = sprite.sprite;

        Transform statsParent = playerSelect.GetComponentInChildren<HorizontalLayoutGroup>().transform.parent;

        for (int i = 0; i < statsParent.childCount; i++)
        {
            RemoveStatSquares(i, statsParent.GetChild(i));
        }
    }

    private void RemoveStatSquares(int index, Transform statTransform)
    {
        int stat = 3;

        switch (index)
        {
            case 0:
                stat = dash;
                break;
            case 1:
                stat = limit;
                break;
            case 2:
                stat = power;
                break;
            case 3:
                stat = cornering;
                break;
        }        

        for (int i = 0; i < 5; i++)
        {
            statTransform.GetChild(i).GetComponent<Image>().enabled = (i < stat);
        }
    }

    public void Pressed()
    {
        CharacterSelectInput selectInput = GameManager.instance.transform.GetChild(eventIndex).GetComponent<CharacterSelectInput>();

        if (!disabled.activeSelf && selectInput.CanSelect)
        {
            holder.MultiplayerEventSystems[eventIndex].SetSelectedGameObject(null);
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
            holder.MultiplayerEventSystems[index].SetSelectedGameObject(null);
            holder.MultiplayerEventSystems[index].SetSelectedGameObject(gameObject);
        }

        alreadyDeselecting = false;
    }
}
