using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAction : MonoBehaviour
{
    [SerializeField] private bool looping = false;
    public bool Looping { get { return looping; } }
}
