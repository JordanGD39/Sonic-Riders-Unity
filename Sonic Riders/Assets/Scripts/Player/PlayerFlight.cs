using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlight : MonoBehaviour
{
    private CharacterStats charStats;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private HUD hud;

    public bool Flying { get; set; } = false;
    public float VerticalRotation { get; set; } = 0;

    [SerializeField] private float flightSpeedLoss = 3;
    [SerializeField] private float turnMultiplier = 0.5f;
    private float cornering;
    private float rotationAmount = 0;
    private float vertcialRotAmount = 0;
    private bool canCheck = false;
    [SerializeField] private float airGain = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        charStats = GetComponent<CharacterStats>();

        if (!charStats.TypeCheck(type.FLIGHT))
        {
            enabled = false;
        }

        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        cornering = charStats.BoardStats.Cornering * turnMultiplier;
        hud = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Flying)
        {
            charStats.Air += airGain;

            if (playerMovement.Speed > 0)
            {
                playerMovement.Speed -= flightSpeedLoss * Time.deltaTime;
            }

            hud.UpdateSpeedText(playerMovement.Speed);

            vertcialRotAmount = VerticalRotation * cornering;
            vertcialRotAmount *= Time.deltaTime;
            rotationAmount = playerMovement.TurnAmount * cornering;
            rotationAmount *= Time.deltaTime;

            transform.GetChild(0).Rotate(vertcialRotAmount, rotationAmount, 0);

            if ((playerMovement.Speed < 10 || playerMovement.Grounded) && canCheck)
            {               
                Quaternion otherRot = transform.GetChild(0).localRotation;
                otherRot.x = 0;
                otherRot.z = 0;
                transform.GetChild(0).localRotation = otherRot;

                Flying = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Flying)
        {
            rb.velocity = transform.GetChild(0).forward * playerMovement.Speed;
        }
    }

    public void CanCheckGrounded()
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
        Flying = true;
        transform.GetChild(0).forward = portal.forward;

        playerMovement.Speed = 50;
    }
}
