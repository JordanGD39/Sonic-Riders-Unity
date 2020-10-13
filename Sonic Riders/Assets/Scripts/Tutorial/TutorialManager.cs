using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Lesson[] lessons;
    private int lessonIndex = 0;

    private PlayerMovement player;
    public PlayerMovement Player { set { player = value; } }

    private BigCanvasUI bigCanvasUI;

    private InputAction prevInputAction;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();
        AddActionPerformed();
    }

    private void AddActionPerformed()
    {
        prevInputAction = null;
        destination = Vector3.zero;

        Lesson currLesson = lessons[lessonIndex];

        InputAction action = currLesson.Action;

        if (action != null)
        {
            action.performed += ctx => LessonDone();
            prevInputAction = action;
        }

        destination = currLesson.Destination;

        bigCanvasUI.ShowTutorialText(currLesson.TextField);
    }

    private void LessonDone()
    {
        if (prevInputAction != null)
        {
            prevInputAction.performed -= ctx => LessonDone();
        }

        bigCanvasUI.RemovePopup();

        lessonIndex++;

        if (lessonIndex >= lessons.Length)
        {
            return;
        }

        Invoke("AddActionPerformed", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (lessons[lessonIndex].Falling && !player.Grounded)
        {
            LessonDone();
        }*/
    }
}
