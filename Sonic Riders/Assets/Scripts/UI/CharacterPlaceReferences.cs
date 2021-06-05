using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPlaceReferences : MonoBehaviour
{
    [SerializeField] private Transform placingParent;
    public Transform PlacingParent { get { return placingParent; } }

    [SerializeField] private Image portrait;
    public Image Portrait { get { return portrait; } }

    [SerializeField] private TextMeshProUGUI characterName;
    public TextMeshProUGUI CharacterName { get { return characterName; } }

    [SerializeField] private TextMeshProUGUI time;
    public TextMeshProUGUI TimeText { get { return time; } }
}
