using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostOnAnimation : MonoBehaviour
{
    private PlayerBoost playerBoost;

    private void Start()
    {
        playerBoost = GetComponentInParent<PlayerBoost>();
    }

    public void BoostNow()
    {
        playerBoost.Boost();
    }
}
