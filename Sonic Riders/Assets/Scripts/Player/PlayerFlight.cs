using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlight : MonoBehaviour
{
    private CharacterStats charStats;
    private PlayerMovement playerMovement;
    private PlayerTricks playerTricks;
    private TurbulenceGenerator turbulenceGenerator;
    private Rigidbody rb;
    private HUD hud;
    private Animator canvasAnim;
    private AudioManagerHolder audioHolder;
    private bool flying = false;
    public bool Flying { get { return flying; } }
    public float VerticalRotation { get; set; } = 0;

    [SerializeField] private float flightSpeedLoss = 3;
    [SerializeField] private float turnMultiplier = 0.5f;
    private float cornering;
    private float rotationAmount = 0;
    private float vertcialRotAmount = 0;
    private bool canCheck = false;
    [SerializeField] private float airGain = 0.02f;
    private float flightSpeed = 0;

    public void GiveCanvasHud()
    {
        charStats = GetComponent<CharacterStats>();

        if (!charStats.TypeCheck(type.FLIGHT))
        {
            enabled = false;
        }

        playerMovement = GetComponent<PlayerMovement>();
        turbulenceGenerator = GetComponent<TurbulenceGenerator>();
        playerTricks = GetComponent<PlayerTricks>();
        rb = GetComponent<Rigidbody>();
        cornering = charStats.BoardStats.Cornering * turnMultiplier;

        audioHolder = GetComponent<AudioManagerHolder>();

        if (charStats.IsPlayer)
        {
            hud = charStats.Canvas.GetComponent<HUD>();
            canvasAnim = hud.GetComponent<Animator>();
        }       
    }
    
    // Update is called once per frame
    void Update()
    {
        if (flying)
        {
            if (!charStats.BoardStats.RingsAsAir)
                charStats.Air += airGain * Time.deltaTime;

            if (playerMovement.Speed > 0)
            {
                playerMovement.Speed -= flightSpeedLoss * Time.deltaTime;
            }

            if (!playerMovement.Grounded)
            {
                flightSpeed = playerMovement.Speed;
            }

            hud.UpdateSpeedText(playerMovement.Speed);

            vertcialRotAmount = VerticalRotation * cornering;
            vertcialRotAmount *= Time.deltaTime;
            rotationAmount = playerMovement.TurnAmount * cornering;
            rotationAmount *= Time.deltaTime;

            transform.GetChild(0).Rotate(vertcialRotAmount, rotationAmount, 0);

            if (playerMovement.Speed < 13 || playerMovement.Grounded && canCheck)
            {               
                Quaternion otherRot = transform.GetChild(0).localRotation;
                otherRot.x = 0;
                otherRot.z = 0;
                transform.GetChild(0).localRotation = otherRot;

                turbulenceGenerator.ResumeGeneration(transform.position);

                if (playerMovement.Grounded)
                {
                    playerMovement.Speed = flightSpeed;
                }

                flying = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (flying)
        {
            rb.velocity = transform.GetChild(0).forward * playerMovement.Speed;
        }
    }

    public void FallingOffRamp(float perfectJump)
    {
        if (transform.localPosition.z > perfectJump && !flying)
        {
            CanCheckGrounded();
            flying = true;
            transform.parent = null;

            turbulenceGenerator.PauseGeneration();
        }
    }

    private void CanCheckGrounded()
    {
        canCheck = false;
        StartCoroutine("CheckGrounded");
    }

    private IEnumerator CheckGrounded()
    {
        yield return new WaitForSeconds(0.5f);
        canCheck = true;
    }
    
    public void IncreaseFlightSpeed(Transform portal)
    {
        if (playerTricks.CanDoTricks)
        {
            playerTricks.Landed(false);
        }

        canCheck = true;
        flying = true;
        transform.GetChild(0).forward = portal.forward;

        playerMovement.Speed = 67;
        audioHolder.SfxManager.Play(Constants.SoundEffects.flightRing);

        if (charStats.IsPlayer)
        {
            canvasAnim.Play("BoostCircle");
        }
    }
}
