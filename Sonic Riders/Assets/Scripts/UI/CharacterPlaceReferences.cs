using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPlaceReferences : MonoBehaviour
{
    [SerializeField] private Transform placingParent;
    public Transform PlacingParent { get { return placingParent; } }

    [SerializeField] private Image portrait;
    public Image Portrait { get { return portrait; } }

    [SerializeField] private Text characterName;
    public Text CharacterName { get { return characterName; } }

    [SerializeField] private Text time;
    public Text TimeText { get { return time; } }
}
