using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private TurbulenceGenerator turbulenceGenerator;
    private TurbulenceRider turbulenceRider;
    private AudioManagerHolder audioHolder;
    private PlayerAnimationHandler playerAnimation;
    private CharacterStats charStats;
    private BoardStats stats;

    private Animator canvasAnim;
    [SerializeField] private bool boosting = false;
    public bool Boosting { get { return boosting; } set { boosting = value; } }
    public bool Attacking { get; set; } = false;

    [SerializeField] private bool attackAnim = true;
    public bool AttackAnim { get { return attackAnim; } }
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
    [SerializeField] private float attackRadius = 2.5f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private string boostSound = string.Empty;
    private Transform prevAttackingPlayer;
    private Transform triggerCol;

    public void GiveAnim()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        charStats = GetComponent<CharacterStats>();

        if (boostSound == string.Empty || charStats.SuperForm)
        {
            boostSound = Constants.SoundEffects.boost;
        }

        stats = charStats.BoardStats;
        audioHolder = GetComponent<AudioManagerHolder>();
        turbulenceGenerator = GetComponent<TurbulenceGenerator>();
        turbulenceRider = GetComponent<TurbulenceRider>();
        triggerCol = GetComponentInChildren<PlayerTrigger>().transform;

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

        if (boosting)
        {
            if (!playerMovement.Grounded || charStats.OffRoad)
            {
                boosting = false;
                Attacking = false;
                playerAnimation.Anim.ResetTrigger("Punch");
                playerAnimation.Anim.ResetTrigger("LeftPunch");
                prevAttackingPlayer = null;
                playerAnimation.Anim.SetBool("Boosting", false);

                if (attackCol != null)
                {
                    attackCol.SetActive(false);
                }

                if (attackColShow != null)
                {
                    attackColShow.SetActive(false);
                }

                if (charStats.IsPlayer)
                {
                    startPuttingBackCameraPos = true;
                }
            }

            if (Attacking && attackAnim)
            {
                Vector3 localExtraPos = transform.GetChild(0).TransformVector(new Vector3(0,0, attackDistance));

                int layerMask = LayerMask.GetMask("Player");

                Collider[] colliders = Physics.OverlapSphere(transform.position + localExtraPos, attackRadius, layerMask);

                List<Collider> colList = new List<Collider>();

                //float closestDistance = 999;
                //int closestIndex = -1;

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].transform != triggerCol && colliders[i].CompareTag(Constants.Tags.triggerCol) && colliders[i].transform != prevAttackingPlayer)
                    {
                        colList.Add(colliders[i]);
                        /*
                        float distance = (transform.position - colliders[i].transform.position).sqrMagnitude;

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIndex = i;
                        }*/
                    }                    
                }

                if (colList.Count > 0)
                {
                    playerAnimation.Anim.SetTrigger("Attack");
                }

                /*if (closestIndex < 0)
                {
                    return;
                }

                Transform closestPlayer = colliders[closestIndex].transform;                

                Debug.Log(closestPlayer.GetComponentInParent<CharacterStats>().gameObject.name + " col: " + closestPlayer);

                prevAttackingPlayer = closestPlayer;

                Vector3 dirToPlayer = (closestPlayer.position - transform.position).normalized;
                Vector3 localDirToObject = transform.GetChild(0).TransformDirection(dirToPlayer);
                localDirToObject.y = 0;

                transform.GetChild(0).forward = transform.GetChild(0).InverseTransformDirection(localDirToObject);*/
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
        if (charStats.DisableAllFeatures || charStats.GetCurrentBoost() == 0 ||(!playerMovement.Grounded && !playerMovement.CanBoostInAir) || boosting || charStats.Air < charStats.GetCurrentBoostDepletion() || turbulenceRider.InTurbulence)
        {
            return;
        }

        if (charStats.SuperForm)
        {
            charStats.ResetSuperRotation();
        }

        boosting = true;

        if (!playerMovement.CanBoostInAir && playerAnimation.Anim != null)
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

            if (attackCol != null)
            {
                attackCol.SetActive(false);
            }
        }        

        playerMovement.FallToTheGround = false;
        charStats.Air -= charStats.GetCurrentBoostDepletion();

        StopCoroutine("BoostCooldown");
        StartCoroutine("BoostCooldown");

        if (playerMovement.Speed < charStats.GetCurrentBoost() || playerMovement.CanBoostInAir)
        {
            playerMovement.Speed = charStats.GetCurrentBoost();
        }
        else
        {
            playerMovement.Speed += 5;
        }

        string soundToPlay = playerMovement.CanBoostInAir ? Constants.SoundEffects.boost : boostSound;

        audioHolder.SfxManager.Play(soundToPlay);

        //turbulenceGenerator.StartPathGeneration();

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
        playerAnimation.Anim.ResetTrigger("Punch");
        playerAnimation.Anim.ResetTrigger("LeftPunch");
        prevAttackingPlayer = null;

        if (attackCol != null)
        {
            attackCol.SetActive(false);
        }

        if (attackColShow != null)
        {
            attackColShow.SetActive(false);
        }

        playerAnimation.Anim.SetBool("Boosting", false);

        if (charStats.IsPlayer)
        {
            startPuttingBackCameraPos = true;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 localExtraPos = transform.GetChild(0).TransformVector(new Vector3(0, 0, attackDistance));
        Gizmos.DrawWireSphere(transform.position + localExtraPos, attackRadius);
    }
}
