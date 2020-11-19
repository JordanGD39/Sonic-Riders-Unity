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
    [SerializeField] private Image boardImageFinal;
    public Image BoardImageFinal { get { return boardImageFinal; } }
    [SerializeField] private Text boardText;
    public Text BoardText { get { return boardText; } }

    [SerializeField] private Transform statsParent;
    public Transform StatsParent { get { return statsParent; } }
    [SerializeField] private Transform typeParent;
    public Transform TypeParent { get { return typeParent; } }

    [SerializeField] private GameObject pointersParent;
    public GameObject PointersParent { get { return pointersParent; } }

    [SerializeField] private GameObject infoPanel;
    public GameObject InfoPanel { get { return infoPanel; } }
    [SerializeField] private Text infoText;
    public Text InfoText { get { return infoText; } }

    private string oldInfoText = "";
    public string OldInfoText { get { return oldInfoText; } }

    [SerializeField] private Text fullName;
    public Text FullName { get { return fullName; } }

    private void Start()
    {
        oldInfoText = infoText.text;
        infoPanel.SetActive(false);
        notJoinedPanel.SetActive(true);
        joinedPanel.SetActive(false);
        charImage.gameObject.SetActive(true);
        boardImage.gameObject.SetActive(true);
        //boardImage.transform.parent.gameObject.SetActive(false);
        boardImageFinal.gameObject.SetActive(false);
        pointersParent.SetActive(true);
        boardText.gameObject.SetActive(true);
    }
}
