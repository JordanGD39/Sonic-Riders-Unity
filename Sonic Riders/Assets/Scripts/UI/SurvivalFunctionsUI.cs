using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalFunctionsUI : MonoBehaviour
{
    [SerializeField] private Text[] survivalScores;
    [SerializeField] private Image[] survivalIcons;
    private List<RectTransform> survivalIconsRect = new List<RectTransform>();
    [SerializeField] private Image[] survivalColors;
    [SerializeField] private Image survivalLeader;
    private Sprite emeraldSprite;

    public bool ReadyToChange { get; set; } = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
        emeraldSprite = survivalLeader.sprite;

        for (int i = 0; i < survivalIcons.Length; i++)
        {
            survivalIcons[i].gameObject.SetActive(false);
            survivalScores[i].transform.parent.gameObject.SetActive(false);
            Color gray = Color.gray;
            gray.a = 0.39f;
            survivalColors[i].color = gray;
            survivalIconsRect.Add(survivalIcons[i].GetComponent<RectTransform>());
        }

        ReadyToChange = true;
    }

    public void ChangeSurvivalColor(int colorIndex, Color color)
    {
        survivalColors[colorIndex].color = color;
    }

    public void ChangeIcons(int iconIndex, Sprite sprite)
    {
        survivalIcons[iconIndex].sprite = sprite;

        if (survivalIcons[iconIndex].sprite != survivalLeader.sprite)
        {
            survivalIcons[iconIndex].gameObject.SetActive(true);
        }
    }

    public void UpdateIconDistance(int charIndex, float y, float x, float xMultiplier)
    {
        float extraDistance = Mathf.Abs(x) - (20 * xMultiplier);

        if (extraDistance < 0)
        {
            extraDistance = 0;
        }

        if (y > 0)
        {
            extraDistance = -extraDistance;
        }

        survivalIconsRect[charIndex].localPosition = new Vector2(0, Mathf.Clamp(y - extraDistance, -150, 150));
    }

    public void UpdateLeader(Sprite leader)
    {
        if (leader == null)
        {
            survivalLeader.sprite = emeraldSprite;

            for (int i = 0; i < survivalIcons.Length; i++)
            {
                survivalIconsRect[i].localPosition = new Vector2(0, -150);
                survivalIcons[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < survivalIcons.Length; i++)
            {
                survivalIconsRect[i].localPosition = new Vector2(0, -150);
                survivalIcons[i].gameObject.SetActive(true);

                if (survivalIcons[i].sprite == leader)
                {
                    survivalIcons[i].gameObject.SetActive(false);
                }
            }

            survivalLeader.sprite = leader;
        }
    }

    public void ShowSurvivalScores(int index)
    {
        survivalScores[index].transform.parent.gameObject.SetActive(true);
    }

    public void UpdateSurvivalScore(int index, int score)
    {
        survivalScores[index].text = score.ToString("00");
    }
}
