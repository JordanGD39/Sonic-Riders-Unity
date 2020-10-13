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
    [SerializeField] private bool boosting = false;
    public bool Boosting { get { return boosting; } set { boosting = value; } }

    [SerializeField] private Vector3 camPos;
    [SerializeField] private float camSpeedBoost = 5;
    [SerializeField] private float camSpeedStopBoosting = 2;
    private float offroadTimer = 1;

    private bool startCameraPos = false;
    private bool startPuttingBackCameraPos = false;

    private ParticleSystem ps;

    public void GiveAnim()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        charStats = GetComponent<CharacterStats>();
        stats = charStats.BoardStats;
        audioHolder = GetComponent<AudioManagerHolder>();
        if (charStats.IsPlayer)
            canvasAnim = charStats.Canvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (charStats == null)
        {
            return;
        }

        if (charStats.OffRoad)
        {
            offroadTimer -= Time.deltaTime;
        }
        else
        {
            offroadTimer = 1;
        }

        if (boosting && (!playerMovement.Grounded || offroadTimer <= 0))
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
            charStats.Cam.localPosition = Vector3.MoveTowards(charStats.Cam.localPosition, charStats.CamStartPos, step);

            if (charStats.Cam.localPosition == charStats.CamStartPos)
            {
                startPuttingBackCameraPos = false;
            }
        }

        if (startCameraPos)
        {
            float step = camSpeedBoost * Time.deltaTime;
            charStats.Cam.localPosition = Vector3.MoveTowards(charStats.Cam.localPosition, camPos, step);

            if (charStats.Cam.localPosition == camPos)
            {
                startCameraPos = false;
            }
        }
    }

    public void CheckBoost()
    {
        if (charStats.DisableAllFeatures || !((playerMovement.Grounded || playerGrind.Grinding) && !boosting && charStats.Air > charStats.GetCurrentBoostDepletion()))
        {
            return;
        }

        boosting = true;

        if (!playerGrind.Grinding && playerAnimation.Anim != null)
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
        charStats.Air -= charStats.GetCurrentBoostDepletion();

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

        if (charStats.IsPlayer)
        {
            canvasAnim.Play("BoostCircle");
        }
    }

    private IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(charStats.GetCurrentBoostTime());

        boosting = false;

        if (charStats.IsPlayer)
        {
            startPuttingBackCameraPos = true;
        }
    }
}
