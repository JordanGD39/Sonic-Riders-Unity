using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigCanvasUI : MonoBehaviour
{
    [SerializeField] private GameObject characterPlacePref;
    [SerializeField] private Transform characterPlaceParent;
    [SerializeField] private Text timeText;
    private float timer = 0;

    private ChangePlace changePlace;

    // Start is called before the first frame update
    void Start()
    {
        changePlace = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponentInChildren<ChangePlace>();
        characterPlaceParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        timer += Time.deltaTime;



        timeText.text = 
    }

    public void PostPlacings(List<PlayerCheckpoints> players)
    {
        characterPlaceParent.gameObject.SetActive(true);

        for (int i = 0; i < players.Count; i++)
        {
            GameObject playerPlace = Instantiate(characterPlacePref, characterPlaceParent, false);

            playerPlace.GetComponent<RectTransform>().anchoredPosition = new Vector2(410 + (i * 20), -190 - (i * 50));

            CharacterPlaceReferences references = playerPlace.GetComponent<CharacterPlaceReferences>();

            Transform placingParent = references.PlacingParent;

            changePlace.ChangeImageReferences(placingParent.GetChild(1).GetComponent<Image>(), placingParent.GetChild(2), placingParent.GetChild(0).gameObject, i);
            changePlace.UpdatePlacing();

            CharacterStats stats = players[i].CharStats;

            references.Portrait.sprite = stats.Portrait;

            RectTransform portraitTransform = references.Portrait.GetComponent<RectTransform>();
            portraitTransform.anchoredPosition = new Vector2(portraitTransform.anchoredPosition.x, portraitTransform.anchoredPosition.y + stats.ExtraY);

            references.CharacterName.text = stats.CharacterName;
        }
    }
}
