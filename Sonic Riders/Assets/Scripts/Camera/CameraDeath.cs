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

    public void GiveCanvasAnim()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerCheckpoints = GetComponentInParent<PlayerCheckpoints>();
        charStats = GetComponentInParent<CharacterStats>();
        player = playerMovement.transform;
        startPos = transform.localPosition;
        prevParent = transform.parent;
        playerRb = player.GetComponent<Rigidbody>();
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
        transform.parent = null;
        followPlayer = true;
        canvasAnim.Play("DeathFadeIn");
        StartCoroutine("WaitForRespawn");
    }

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        canvasAnim.Play("DeathFadeOut");
        followPlayer = false;
        Transform checkPoint = playerCheckpoints.RaceManagerScript.transform.GetChild(playerCheckpoints.CurrCheckpoint).GetChild(0);
        player.transform.GetChild(0).forward = checkPoint.parent.forward;
        player.position = checkPoint.position;
        playerRb.velocity = Vector3.zero;
        transform.parent = prevParent;
        transform.localPosition = startPos;
        transform.localRotation = new Quaternion(0, 0, 0, transform.localRotation.w);

        if (!charStats.BoardStats.RingsAsAir)
        {
            charStats.Rings -= 50;
        }
    }
}
