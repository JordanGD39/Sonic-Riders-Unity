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
    public bool Attacking { get; set; } = false;

    [SerializeField] private Vector3 camPos;
    [SerializeField] private float camSpeedBoost = 5;
    [SerializeField] private float camSpeedStopBoosting = 2;
    private float offroadTimer = 1;

    private bool startCameraPos = false;
    private bool startPuttingBackCameraPos = false;

    private ParticleSystem ps;
    [SerializeField] private GameObject attackCol;
    [SerializeField] private GameObject attackColShow;
    public GameObject AttackCol { get { return attackCol; } }
    [SerializeField] private float attackDelay = 0.25f;

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

        if (boosting && (!playerMovement.Grounded || charStats.OffRoad))
        {
            boosting = false;
            Attacking = false;
            playerAnimation.Anim.SetBool("Boosting", false);
            attackCol.SetActive(false);

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
        if (charStats.DisableAllFeatures || charStats.GetCurrentBoost() == 0 ||(!playerMovement.Grounded && !playerGrind.Grinding) || boosting || charStats.Air < charStats.GetCurrentBoostDepletion())
        {
            return;
        }

        boosting = true;

        if (!playerGrind.Grinding && playerAnimation.Anim != null)
        {
            playerAnimation.StartBoostAnimation(!charStats.SurvivalLeader);
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

        if (!charStats.SurvivalLeader)
        {
            playerAnimation.AlreadySettingAttack = true;
            Attacking = true;
            playerAnimation.Anim.SetBool("Boosting", true);

            if (attackColShow != null)
            {
                attackColShow.SetActive(false);
                Invoke("ShowAttackTrigger", attackDelay);
            }

            attackCol.SetActive(true);
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

    private void ShowAttackTrigger()
    {
        attackColShow.SetActive(true);
        playerAnimation.AlreadySettingAttack = false;
    }

    private IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(charStats.GetCurrentBoostTime());

        Attacking = false;
        boosting = false;
        attackCol.SetActive(false);
        playerAnimation.Anim.SetBool("Boosting", false);

        if (charStats.IsPlayer)
        {
            startPuttingBackCameraPos = true;
        }
    }
}
