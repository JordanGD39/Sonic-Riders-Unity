using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestHandleJoin : MonoBehaviour
{
    public void HandlePlayerJoin(PlayerInput pi)
    {
        pi.GetComponent<PlayerControls>().enabled = true;
    }
}
