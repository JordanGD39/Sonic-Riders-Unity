using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Lesson", menuName = "Lesson")]
public class Lesson : ScriptableObject
{
    [SerializeField] [TextArea] private string inputField;
    public string TextField { get { return inputField; } }

    [SerializeField] private InputActionReference inputAction;
    public InputAction Action { get { return inputAction; } }

    [SerializeField] private Vector3 destination;
    public Vector3 Destination { get { return destination; } }

    [SerializeField] private bool falling = false;
    public bool Falling { get { return falling; } }

    [SerializeField] private bool freezeTime = false;
    public bool FreezeTime { get { return freezeTime; } }

    [SerializeField] private bool jumpingOfRamp = false;
    public bool JumpingOfRamp { get { return jumpingOfRamp; } }

    [SerializeField] private bool flying = false;
    public bool Flying { get { return flying; } }

    [SerializeField] private bool grinding = false;
    public bool Grinding { get { return grinding; } }

    [SerializeField] private int indexSkip = 0;
    public int IndexSkip { get { return indexSkip; } }

    [SerializeField] private bool startCountdown = false;
    public bool StartCountdown { get { return startCountdown; } }
}
