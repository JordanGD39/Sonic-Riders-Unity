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

    // Start is called before the first frame update
    void Start()
    {
        holder = GetComponentInParent<EventSystemHolder>();
        playerSelectParent = holder.GetComponentInChildren<GridLayoutGroup>().transform;
    }

    public void Selected()
    {
        int index = 0;

        for (int i = 0; i < holder.MultiplayerEventSystems.Count; i++)
        {
            if (holder.MultiplayerEventSystems[i].currentSelectedGameObject == gameObject)
            {
                index = i;
            }
        }

        Transform playerSelect = playerSelectParent.GetChild(index);

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

        for (int i = 4; i + 1 > stat; i--)
        {
            statTransform.GetChild(i).GetComponent<Image>().enabled = false;
        }
    }
}
