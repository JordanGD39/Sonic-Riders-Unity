using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringStats : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    public float Speed { get { return speed; } }
    [SerializeField] private float launchSpeed = 10f;
    public float LaunchSpeed { get { return launchSpeed; } }
    [SerializeField] private Transform forward;
    public Transform Forward { get { return forward; } }
}
