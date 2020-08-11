﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    [SerializeField] private float speed = 2;
    [SerializeField] private float time = 0.5f;
    private Vector3 bounceDir;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        bounceDir = (transform.position - collision.ClosestPoint(transform.position)).normalized;
        StartCoroutine("Bounce");
    }

    private IEnumerator Bounce()
    {
        playerMovement.Speed = speed;
        yield return null;
        playerMovement.CantMove = true;
        rb.velocity = bounceDir * speed;
        yield return new WaitForSeconds(time);
        playerMovement.CantMove = false;
        playerMovement.Speed = 0;
    }
}
