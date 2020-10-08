using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrift : MonoBehaviour
{
    private PlayerMovement movement;
    private BoardStats stats;
    private CharacterStats charStats;
    private AudioManagerHolder audioHolder;
    public bool DriftPressed { get; set; } = false;
    public float DriftDir { get; set; } = 0;
    private Animator canvasAnim;

    private float driftTimer = 0;
    [SerializeField] private float brakePower = 30;

    public void GiveAnim()
    {
        movement = GetComponent<PlayerMovement>();
        charStats = GetComponent<CharacterStats>();

        stats = charStats.BoardStats;
        audioHolder = GetComponent<AudioManagerHolder>();
        if (charStats.IsPlayer)
            canvasAnim = charStats.Canvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement == null)
        {
            return;
        }

        if (movement.Grounded && charStats.Air > 0 && !charStats.DisableAllFeatures)
        {
            if (DriftPressed)
            {
                if (movement.TurnAmount != 0 || driftTimer > 0)
                {
                    Drift();
                }
                else
                {
                    //Braking
                    if (movement.Speed > 0)
                    {
                        movement.Speed -= brakePower * Time.deltaTime;
                    }
                    else if (movement.Speed < 0)
                    {
                        movement.Speed += brakePower * Time.deltaTime;
                    }
                }
                
                return;
            }
        }

        if (!DriftPressed && driftTimer > 1)
        {
            //charStats.Cam.localRotation = new Quaternion(0, 0, 0, charStats.Cam.localRotation.w);
            driftTimer = 0;
            movement.FallToTheGround = false;
            if (movement.Speed < stats.Boost[charStats.Level])
            {
                movement.Speed = stats.Boost[charStats.Level];
            }
            else
            {
                movement.Speed += 5;
            }            
            movement.DriftBoost = true;

            audioHolder.SfxManager.Play(Constants.SoundEffects.boost);

            if (charStats.IsPlayer)
            {
                canvasAnim.Play("BoostCircle");
            }
        }

        movement.Drifting = false;
        driftTimer = 0;
        if (!movement.DriftBoost)
        {
            DriftDir = 0;
        }        
    }

    private void Drift()
    {
        if (movement.Speed <= 0 || charStats.OffRoad)
        {
            movement.Drifting = false;
            driftTimer = 0;
            return;
        }

        if (DriftDir == 0)
        {
            if (movement.TurnAmount > 0)
            {
                DriftDir = 1;
            }
            else
            {
                DriftDir = -1;
            }
        }

        Vector3 camLook = transform.GetChild(0).TransformVector(new Vector3(DriftDir * 0.8f, 1.5f, 1));

        charStats.Cam.LookAt(transform.GetChild(0).position + camLook);

        driftTimer += Time.deltaTime;
        movement.Drifting = true;
        movement.Speed -= 4 * Time.deltaTime;
        charStats.Air -= charStats.GetCurrentAirLoss() * 2 * Time.deltaTime;
    }
}
