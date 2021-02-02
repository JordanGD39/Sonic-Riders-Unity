using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeath : MonoBehaviour
{
    private bool followPlayer = false;
    private Transform player;
    private Rigidbody playerRb;
    private Transform prevParent;
    private PlayerMovement playerMovement;
    private PlayerCheckpoints playerCheckpoints;
    private CharacterStats charStats;
    private Vector3 startPos;
    private Animator canvasAnim;

    [SerializeField] private float timeToRespawn = 3;
    private PlayerAnimationHandler playerAnimation;
    private SurvivalManager survivalManager;

    public void GiveCanvasAnim()
    {
        if (GameManager.instance.GameMode == GameManager.gamemode.SURVIVAL)
        {
            survivalManager = FindObjectOfType<SurvivalManager>();
        }

        playerMovement = GetComponentInParent<PlayerMovement>();
        charStats = GetComponentInParent<CharacterStats>();
        player = playerMovement.transform;
        playerCheckpoints = player.GetComponent<PlayerCheckpoints>();
        startPos = transform.localPosition;
        prevParent = transform.parent;
        playerRb = player.GetComponent<Rigidbody>();
        playerAnimation = player.GetComponent<PlayerAnimationHandler>();
        canvasAnim = charStats.Canvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            if (transform.rotation.eulerAngles.x < 90)
            {
                transform.LookAt(player);
            }
        }
    }

    public void StartFollow()
    {
        if (followPlayer)
        {
            return;
        }

        transform.parent = null;
        followPlayer = true;
        charStats.DisableAllFeatures = true;
        playerAnimation.Anim.SetBool("Dying", true);
        playerAnimation.Anim.SetBool("GotHit", true);
        playerMovement.JustDied = true;
        playerMovement.GoUnderSea(false);
        canvasAnim.Play("DeathFadeIn");

        if (survivalManager != null)
        {
            survivalManager.MoveChaosEmerald(player.gameObject);
        }

        StartCoroutine("WaitForRespawn");
    }

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        playerAnimation.Anim.SetBool("Dying", false);        
        canvasAnim.Play("DeathFadeOut");
        followPlayer = false;
        int checkPointIndex = playerCheckpoints.CurrCheckpoint;

        bool checkpointOutOfBounds = checkPointIndex > playerCheckpoints.RaceManagerScript.transform.childCount - 1;

        if (checkpointOutOfBounds)
        {
            checkPointIndex = 0;
        }

        Transform checkPoint = playerCheckpoints.RaceManagerScript.transform.GetChild(checkPointIndex).GetChild(0);
        player.GetChild(0).up = Vector3.up;
        player.GetChild(0).forward = checkPoint.parent.forward;
        player.position = checkPoint.position;

        playerRb.velocity = Vector3.zero;
        playerMovement.Speed = 0;

        if (charStats.IsPlayer)
        {
            charStats.Hud.UpdateSpeedText(0);
        }

        transform.parent = prevParent;
        transform.localPosition = startPos;
        transform.localRotation = new Quaternion(0, 0, 0, transform.localRotation.w);
        
        playerMovement.AboveSea(false);

        if (!charStats.BoardStats.RingsAsAir)
        {
            charStats.Rings -= 50;
        }
    }
}
