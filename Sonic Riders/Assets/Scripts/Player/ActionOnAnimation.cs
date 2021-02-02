using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOnAnimation : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerBoost playerBoost;
    private PlayerMovement playerMovement;
    private PlayerTricks playerTricks;
    private AudioManagerHolder audioManagerHolder;
    private PlayerAnimationHandler playerAnimation;
    private PlayerPunchObstacle playerPunchObstacle;
    private PlayerCheckpoints playerCheckpoints;
    private CharacterStats characterStats;
    private Rigidbody rb;

    private bool gainAir = false;
    private float airGainSpeed = 32;
    private float ringGainSpeed = 2;

    [SerializeField] private SkinnedMeshRenderer normalRenderer;
    [SerializeField] private SkinnedMeshRenderer superRenderer;

    private void Start()
    {
        if (normalRenderer != null)
        {
            normalRenderer.material = new Material(normalRenderer.material);
            superRenderer.material = new Material(superRenderer.material);
        }
       
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerBoost = playerMovement.GetComponent<PlayerBoost>();
        audioManagerHolder = playerMovement.GetComponent<AudioManagerHolder>();
        playerTricks = playerMovement.GetComponent<PlayerTricks>();
        playerAnimation = playerMovement.GetComponent<PlayerAnimationHandler>();
        playerPunchObstacle = playerMovement.GetComponent<PlayerPunchObstacle>();
        playerCheckpoints = playerMovement.GetComponent<PlayerCheckpoints>();
        characterStats = playerMovement.GetComponent<CharacterStats>();
        rb = playerMovement.GetComponent<Rigidbody>();
    }

    private IEnumerator StartGainingAir()
    {
        while (gainAir)
        {
            if (!playerMovement.OnWater)
            {
                gainAir = false;
            }

            characterStats.Air += characterStats.BoardStats.RingsAsAir ? ringGainSpeed * Time.deltaTime : airGainSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public void StopShining()
    {
        if (characterStats.Rings == 0)
        {
            if (playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("TransformIntoSuper"))
            {
                playerAnimation.Anim.Play("TransformBack");
            }

            return;
        }

        superRenderer.enabled = false;
        characterStats.SuperModel.SetActive(true);
        playerAnimation.ChangeAnimForSuperForm(false);
        Invoke("TurnOnSuperRenderer", 0.3f);
    }

    public void TransformBack()
    {
        if (characterStats.Rings > 0)
        {
            if (playerAnimation.Anim.GetCurrentAnimatorStateInfo(0).IsName("TransformBack"))
            {
                playerAnimation.Anim.Play("TransformationComplete");
            }

            return;
        }

        normalRenderer.enabled = false;
        characterStats.Model.SetActive(true);
        playerAnimation.ChangeAnimForSuperForm(true);
        Invoke("TurnOnNormalRenderer", 0.3f);
    }

    private void TurnOnNormalRenderer()
    {
        characterStats.SuperModel.SetActive(false);
        normalRenderer.enabled = true;
    }

    private void TurnOnSuperRenderer()
    {
        characterStats.Model.SetActive(false);
        superRenderer.enabled = true;
    }

    public void Swimming()
    {
        if (!gainAir)
        {
            gainAir = true;
            StartCoroutine("StartGainingAir");
        }
    }

    public void StopSwimming()
    {
        characterStats.DisableAllFeatures = false;
        playerMovement.Speed = 30;
        rb.velocity = playerMovement.transform.GetChild(0).forward * 30;
        playerMovement.JustDied = false;
        gainAir = false;
        StopCoroutine("StartGainingAir");
    }

    public void BoostNow()
    {
        if (!playerMovement.Grounded)
        {
            playerBoost.Boosting = false;
            return;
        }

        playerBoost.Boost(); 
    }

    public void PunchDone()
    {
        playerAnimation.Anim.SetBool("Punching", false);
    }

    public void SuperCheck()
    {
        if (characterStats.SuperForm && !characterStats.SuperModel.activeSelf && characterStats.Air > 0)
        {
            characterStats.ChangeModel(true);
            playerAnimation.ChangeAnimForSuperForm(false);
        }
    }

    public void PlayTrickSound()
    {
        audioManagerHolder.SfxManager.StopPlaying(Constants.SoundEffects.trick);
        audioManagerHolder.SfxManager.Play(Constants.SoundEffects.trick);
    }

    public void CountTrick()
    {
        //Debug.Log("YESSS");
        playerTricks.TrickCountUp();
        playerTricks.CanLand = true;
    }

    public void CantLand()
    {
        playerTricks.CanLand = false;
        PlayTrickSound();
    }

    public void GotHitFalse()
    {
        playerAnimation.Anim.ResetTrigger("Moved");
        playerAnimation.Anim.SetBool("GotHit", false);
    }

    public void HitDone()
    {
        //If player won the race
        if (playerCheckpoints.FinishedAllLaps)
        {
            return;
        }

        playerMovement.JustDied = false;
        playerMovement.CantMove = false;
        characterStats.DisableAllFeatures = false;
    }
}
