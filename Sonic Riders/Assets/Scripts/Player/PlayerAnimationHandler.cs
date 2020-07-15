using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerGrind playerGrind;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerGrind = GetComponent<PlayerGrind>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", playerMovement.Speed * 0.03f);
        anim.SetBool("Grinding", playerGrind.Grinding);
    }

    public void StartBoostAnimation()
    {
        anim.SetTrigger("Boost");
    }
}
