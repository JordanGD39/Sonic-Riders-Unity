using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerAnimationHandler playerAnimation;
    private PlayerGrind playerGrind;
    private CharacterStats charStats;
    private BoardStats stats;

    [SerializeField] private bool boosting = false;
    public bool Boosting { get { return boosting; } }
    public bool BoostPressed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
        playerAnimation = GetComponent<PlayerAnimationHandler>();
        charStats = GetComponent<CharacterStats>();
        stats = transform.GetChild(1).GetComponent<BoardStats>();
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
            playerAnimation.StartBoostAnimation();
        }
    }

    public void Boost()
    {
        BoostPressed = false;
        playerMovement.FallToTheGround = false;
        charStats.Air -= stats.BoostDepletion;
        boosting = true;
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
    }

    private IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(stats.BoostTime);

        boosting = false;
    }
}
