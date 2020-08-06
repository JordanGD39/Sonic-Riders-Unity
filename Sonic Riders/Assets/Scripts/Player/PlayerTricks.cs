using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTricks : MonoBehaviour
{
    public bool CanDoTricks { get; set; } = false;

    [SerializeField] private float turnSpeed = 2;

    private PlayerMovement playerMovement;
    private PlayerSound playerSound;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerSound = GetComponentInChildren<PlayerSound>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanDoTricks)
        {
            if (playerMovement.Grounded)
            {
                CanDoTricks = false;
                return;
            }

            transform.Rotate(Input.GetAxis("Vertical") * turnSpeed, 0, 0);
        }
    }
}
