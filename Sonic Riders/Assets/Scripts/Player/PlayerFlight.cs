using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlight : MonoBehaviour
{
    private CharacterStats charStats;
    private PlayerMovement playerMovement;
    private PlayerTricks playerTricks;
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

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();

        if (!charStats.TypeCheck(type.FLIGHT))
        {
            enabled = false;
        }

        playerMovement = GetComponent<PlayerMovement>();
        playerTricks = GetComponent<PlayerTricks>();
        rb = GetComponent<Rigidbody>();
        cornering = charStats.BoardStats.Cornering * turnMultiplier;
        hud = GameObject.FindGameObjectWithTag(Constants.Tags.canvas).GetComponent<HUD>();
        canvasAnim = hud.GetComponent<Animator>();
        audioHolder = GetComponent<AudioManagerHolder>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (flying)
        {
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

            if (playerMovement.Speed < 10 || playerMovement.Grounded && canCheck)
            {               
                Quaternion otherRot = transform.GetChild(0).localRotation;
                otherRot.x = 0;
                otherRot.z = 0;
                transform.GetChild(0).localRotation = otherRot;

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

        playerMovement.Speed = 50;
        audioHolder.SfxManager.Play(Constants.SoundEffects.flightRing);

        if (playerMovement.IsPlayer)
        {
            canvasAnim.Play("BoostCircle");
        }
    }
}
