﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAnimationHandler playerAnimation;
    private PlayerGrind playerGrind;
    private CharacterStats charStats;
    private BoardStats stats;

    private Animator canvasAnim;

    [SerializeField] private bool boosting = false;
    public bool Boosting { get { return boosting; } set { boosting = value; } }
    public bool BoostPressed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        charStats = GetComponent<CharacterStats>();
        stats = playerMovement.Stats;
        canvasAnim = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boosting && !playerMovement.Grounded)
        {
            boosting = false;
        }

        if (BoostPressed && (playerMovement.Grounded || playerGrind.Grinding) && !boosting /*&& charStats.Air > 0*/)
        {
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
    }

    public void Boost()
    {
        BoostPressed = false;
        playerMovement.FallToTheGround = false;
        charStats.Air -= stats.BoostDepletion;        
        StopCoroutine("BoostCooldown");
        StartCoroutine("BoostCooldown");

        if (playerMovement.Speed < stats.Boost[charStats.Level])
        {
            playerMovement.Speed = stats.Boost[charStats.Level];
        }
        else
        {
            playerMovement.Speed += 5;
        }

        if (playerMovement.IsPlayer)
        {
            canvasAnim.Play("BoostCircle");
        }
    }

    private IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(stats.BoostTime);

        boosting = false;
    }
}
