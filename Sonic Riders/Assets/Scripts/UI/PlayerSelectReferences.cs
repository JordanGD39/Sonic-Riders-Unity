using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectReferences : MonoBehaviour
{
    [SerializeField] private GameObject notJoinedPanel;
    public GameObject NotJoinedPanel { get { return notJoinedPanel; } }
    [SerializeField] private GameObject joinedPanel;
    public GameObject JoinedPanel { get { return joinedPanel; } }

    [SerializeField] private Image charImage;
    public Image CharacterImage { get { return charImage; } }
    [SerializeField] private Image boardImage;
    public Image BoardImage { get { return boardImage; } }

    [SerializeField] private Transform statsParent;
    public Transform StatsParent { get { return statsParent; } }
    [SerializeField] private Transform typeParent;
    public Transform TypeParent { get { return typeParent; } }

    [SerializeField] private GameObject pointersParent;
    public GameObject PointersParent { get { return pointersParent; } }
}
