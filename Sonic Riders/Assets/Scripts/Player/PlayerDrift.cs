using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrift : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerTricks playerTricks;
    private BoardStats stats;
    private CharacterStats charStats;
    private AudioManagerHolder audioHolder;
    public bool DriftPressed { get; set; } = false;
    public float DriftDir { get; set; } = 0;
    private Animator canvasAnim;
    public bool CantRotateCam { get; set; } = false;

    private float driftTimer = 0;
    private float driftBoost = 1;
    [SerializeField] private float autoDriftTimer = 0;
    [SerializeField] private float brakePower = 30;

    public void GiveAnim()
    {
        charStats = GetComponent<CharacterStats>();
        movement = GetComponent<PlayerMovement>();
        playerTricks = GetComponent<PlayerTricks>();

        stats = charStats.BoardStats;

        if (stats.AutoDrift)
        {
            driftBoost = 0.6f;
        }

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
            if (DriftPressed || autoDriftTimer > 1)
            {
                if (movement.Movement.x != 0 || (driftTimer > 0 && !charStats.BoardStats.AutoDrift))
                {
                    Drift();
                }
                else
                {
                    if (charStats.BoardStats.AutoDrift && autoDriftTimer > 1)
                    {
                        autoDriftTimer = 0;
                        return;
                    }

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
            else
            {
                ResetCameraRotation();
            }
        }
        else
        {
            ResetCameraRotation();
        }

        if ((!DriftPressed || (movement.Movement.x != 0 && charStats.BoardStats.AutoDrift)) && driftTimer > driftBoost)
        {
            //charStats.Cam.localRotation = new Quaternion(0, 0, 0, charStats.Cam.localRotation.w);
            driftTimer = 0;
            autoDriftTimer = 0;
            movement.FallToTheGround = false;
            if (movement.Speed < charStats.GetCurrentBoost())
            {
                movement.Speed = charStats.GetCurrentBoost();
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

        if (charStats.BoardStats.AutoDrift && movement.Grounded && Mathf.Abs(movement.Movement.x) >= movement.Movement.z && movement.Movement.x != 0 && !charStats.OffRoad)
        {
            autoDriftTimer += Time.deltaTime;
            return;
        }

        movement.Drifting = false;
        driftTimer = 0;
        autoDriftTimer = 0;
        if (!movement.DriftBoost)
        {
            DriftDir = 0;
        }        
    }

    private void ResetCameraRotation()
    {
        if (charStats.Cam.localRotation != new Quaternion(0, 0, 0, charStats.Cam.localRotation.w) && !playerTricks.CanDoTricks && !CantRotateCam)
        {
            charStats.Cam.localRotation = Quaternion.RotateTowards(charStats.Cam.localRotation, new Quaternion(0, 0, 0, charStats.Cam.localRotation.w), 200 * Time.deltaTime);
        }
    }

    private void Drift()
    {
        if (movement.Speed <= 0)
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

        Vector3 camLook = transform.GetChild(0).TransformVector(new Vector3(DriftDir * 0.8f, 0, 1));
        Vector3 sum = transform.position + camLook;
        sum.y = transform.position.y;
        Quaternion targetDir = Quaternion.LookRotation(sum - transform.position, charStats.Cam.up);
        charStats.Cam.rotation = Quaternion.Slerp(charStats.Cam.rotation, targetDir, 2 * Time.deltaTime);

        if (!charStats.OffRoad)
        {
            driftTimer += Time.deltaTime;
        }

        movement.Speed -= 4 * Time.deltaTime;
        movement.Drifting = true;
        charStats.Air -= charStats.GetCurrentDriftDepletion() * Time.deltaTime;
    }
}
