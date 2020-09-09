using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioManagerHolder audioHolder;
    private PlayerAnimationHandler playerAnimation;
    private PlayerGrind playerGrind;
    private CharacterStats charStats;
    private BoardStats stats;

    private Animator canvasAnim;
    private Transform cam;
    [SerializeField] private bool boosting = false;
    public bool Boosting { get { return boosting; } set { boosting = value; } }

    [SerializeField] private Vector3 camPos;
    private Vector3 oldCamPos;
    [SerializeField] private float camSpeedBoost = 5;
    [SerializeField] private float camSpeedStopBoosting = 2;

    private bool startCameraPos = false;
    private bool startPuttingBackCameraPos = false;

    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        charStats = GetComponent<CharacterStats>();
        stats = charStats.BoardStats;
        audioHolder = GetComponent<AudioManagerHolder>();
        canvasAnim = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<Animator>();
        cam = Camera.main.transform.parent;
        oldCamPos = cam.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (boosting && (!playerMovement.Grounded || charStats.OffRoad))
        {
            boosting = false;

            if (charStats.IsPlayer)
            {
                startPuttingBackCameraPos = true;
            }
        }

        if (startPuttingBackCameraPos)
        {
            startCameraPos = false;
            float step = camSpeedStopBoosting * Time.deltaTime;
            cam.localPosition = Vector3.MoveTowards(cam.localPosition, oldCamPos, step);

            if (cam.localPosition == oldCamPos)
            {
                startPuttingBackCameraPos = false;
            }
        }

        if (startCameraPos)
        {
            float step = camSpeedBoost * Time.deltaTime;
            cam.localPosition = Vector3.MoveTowards(cam.localPosition, camPos, step);

            if (cam.localPosition == camPos)
            {
                startCameraPos = false;
            }
        }
    }

    public void CheckBoost()
    {
        if (!((playerMovement.Grounded || playerGrind.Grinding) && !boosting && charStats.Air > stats.BoostDepletion))
        {
            return;
        }

        boosting = true;

        if (!playerGrind.Grinding)
        {
            playerAnimation.StartBoostAnimation();
        }
        else
        {
            Boost();
        }
    }

    public void Boost()
    {
        if (charStats.IsPlayer)
        {
            startCameraPos = true;
        }
        
        playerMovement.FallToTheGround = false;
        charStats.Air -= stats.BoostDepletion;

        StopCoroutine("BoostCooldown");
        StartCoroutine("BoostCooldown");     

        if (playerMovement.Speed < charStats.GetCurrentBoost())
        {
            playerMovement.Speed = charStats.GetCurrentBoost();
        }
        else
        {
            playerMovement.Speed += 5;
        }

        audioHolder.SfxManager.Play(Constants.SoundEffects.boost);

        if (playerMovement.IsPlayer)
        {
            canvasAnim.Play("BoostCircle");
        }
    }

    private IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(stats.BoostTime);

        boosting = false;

        if (charStats.IsPlayer)
        {
            startPuttingBackCameraPos = true;
        }
    }
}
