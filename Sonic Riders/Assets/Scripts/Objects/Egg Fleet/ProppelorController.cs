using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ProppelorController : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    [SerializeField] private Transform model;
    [SerializeField] private Transform handle;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float clampedX = 3.5f;
    [SerializeField] private float clampedY = 2.5f;
    [SerializeField] private float clampedNegativeY = -1.6f;
    [SerializeField] private float playerPosY = -1;

    private PlayerMovement playerMovement;
    private Rigidbody playerRb;
    private CharacterStats characterStats;
    private bool followPath = false;
    private float closestDistance;
    private Vector3 endPos;
    private Vector3 startPos;
    private Vector3 startForward;
    private Transform oldCamParent;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startForward = transform.forward;
        endPos = path.path.GetPointAtTime(1, EndOfPathInstruction.Stop);
    }

    // Update is called once per frame
    void Update()
    {
        if (!followPath){ return; }

        closestDistance += playerMovement.Speed / 2 * Time.deltaTime;
        transform.position = path.path.GetPointAtDistance(closestDistance, EndOfPathInstruction.Stop);        

        transform.rotation = path.path.GetRotationAtDistance(closestDistance, EndOfPathInstruction.Stop);

        model.localPosition += new Vector3(playerMovement.Movement.x, playerMovement.Movement.z, 0) * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(model.localPosition.x) > clampedX)
        {
            float x = model.localPosition.x;
            model.localPosition = new Vector3(x > 0 ? clampedX : -clampedX, model.localPosition.y, 0);
        }

        if (model.localPosition.y > clampedY || model.localPosition.y < clampedNegativeY)
        {
            float y = model.localPosition.y;
            model.localPosition = new Vector3(model.localPosition.x, y > 0 ? clampedY : clampedNegativeY, 0);
        }

        float limit = characterStats.GetCurrentLimit() / 2;

        if (playerMovement.Speed > limit + 0.33f)
        {
            playerMovement.Speed -= 6 * Time.deltaTime;
        }
        else if (playerMovement.Speed < limit)
        {
            playerMovement.Speed += characterStats.GetCurrentDash() * Time.deltaTime;
        }

        characterStats.Hud.UpdateSpeedText(playerMovement.Speed);

        if (transform.position == endPos)
        {
            followPath = false;
            playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            playerRb.isKinematic = false;
            playerMovement.transform.GetChild(0).localRotation = transform.rotation;

            playerRb.transform.SetParent(null);
            playerMovement.CantMove = false;
            playerMovement.CanBoostInAir = false;
            characterStats.DontAlign = false;
            characterStats.Cam.SetParent(oldCamParent);
            characterStats.Cam.localPosition = Vector3.up * 1.5f;
            transform.position = startPos;
            transform.forward = startForward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (followPath)
        {
            return;
        }

        playerRb = other.attachedRigidbody;
        playerRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        playerRb.isKinematic = true;

        playerRb.transform.SetParent(handle);
        playerMovement = playerRb.GetComponent<PlayerMovement>();
        playerMovement.transform.GetChild(0).forward = transform.forward;
        playerMovement.CanBoostInAir = true;
        playerMovement.CantMove = true;
        playerMovement.transform.localPosition = Vector3.up * playerPosY;

        characterStats = playerMovement.GetComponent<CharacterStats>();
        characterStats.DontAlign = true;
        oldCamParent = characterStats.Cam.parent;
        characterStats.Cam.SetParent(transform);

        float limit = characterStats.GetCurrentLimit();

        if (playerMovement.Speed < limit)
        {
            playerMovement.Speed = limit;
        }

        followPath = true;
    }
}
