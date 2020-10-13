using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Lesson[] lessons;
    [SerializeField] private int lessonIndex = 0;

    private PlayerMovement player;
    private PlayerTricks playerTricks;
    public PlayerMovement Player { set { player = value; } }

    public StartingLevel StartLevel { get; set; }

    private BigCanvasUI bigCanvasUI;

    private InputAction prevInputAction;
    private bool alreadyIncrementing = false;
    private Vector3 destination = Vector3.zero;

    [SerializeField] private Transform destinationAura;
    private ParticleSystem destinationPs;

    [SerializeField] private GameObject invisibleWall;
    [SerializeField] private int removeWall = 8;

    private System.Action<InputAction.CallbackContext> handler;

    // Start is called before the first frame update
    void Start()
    {
        destinationPs = destinationAura.GetComponentInChildren<ParticleSystem>();
        destinationPs.Stop();

        bigCanvasUI = GameObject.FindGameObjectWithTag(Constants.Tags.bigCanvas).GetComponent<BigCanvasUI>();

        handler = (InputAction.CallbackContext ctx) => LessonDone();
        AddActionPerformed();
    }

    public void GivePlayerComponents(PlayerMovement movement)
    {
        player = movement;
        playerTricks = player.GetComponent<PlayerTricks>();
    }

    private void AddActionPerformed()
    {
        alreadyIncrementing = false;
       
        prevInputAction = null;
        destination = Vector3.zero;

        Lesson currLesson = lessons[lessonIndex];

        InputAction action = currLesson.Action;

        if (action != null)
        {
            prevInputAction = action;
            
            prevInputAction.performed += handler;
        }

        destination = currLesson.Destination;

        if (destination != Vector3.zero)
        {
            destinationAura.position = destination;
            destinationPs.Play();
        }

        Time.timeScale = currLesson.FreezeTime ? 0 : 1;

        bigCanvasUI.ShowTutorialText(currLesson.TextField);
    }

    private void LessonDone()
    {
        if (alreadyIncrementing)
        {
            return;
        }
        
        alreadyIncrementing = true;

        destinationPs.Stop();

        if (prevInputAction != null)
        {            
            prevInputAction.performed -= handler;
        }

        bigCanvasUI.RemovePopup();

        if (lessons[lessonIndex].StartCountdown)
        {
            StartLevel.StartCountDownTutorial();
        }

        if (lessonIndex >= lessons.Length - 1)
        {
            return;
        }

        lessonIndex++;

        if (lessonIndex == removeWall)
        {
            invisibleWall.SetActive(false);
        }       

        Time.timeScale = lessons[lessonIndex].FreezeTime ? 0 : 1;

        StartCoroutine("DelayNewLesson");
    }

    private IEnumerator DelayNewLesson()
    {
        yield return new WaitForSecondsRealtime(0.55f);

        AddActionPerformed();
    }

    // Update is called once per frame
    void Update()
    {
        if (lessonIndex >= lessons.Length)
        {
            return;
        }

        if (lessons[lessonIndex].Falling && !player.Grounded)
        {
            LessonDone();
        }

        if (destination != Vector3.zero && (destination - player.transform.position).sqrMagnitude < 6)
        {
            LessonDone();
        }

        if (lessons[lessonIndex].JumpingOfRamp && playerTricks.CanDoTricks)
        {
            LessonDone();
        }
    }
}
