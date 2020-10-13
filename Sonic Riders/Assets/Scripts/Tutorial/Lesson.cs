using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Lesson", menuName = "Lesson")]
public class Lesson : ScriptableObject
{
    [SerializeField] [TextArea] private string inputField;
    public string TextField { get { return inputField; } }

    public enum advancements { CONFIRM, ACTION, DESTINATION, FALLING, NONE }

    [SerializeField] private advancements advancement = advancements.CONFIRM;
    [SerializeField] private advancements secondAdvancement = advancements.NONE;

    public advancements Advancement { get { return advancement; } }
    public advancements SecondAdvancement { get { return secondAdvancement; } }

    [SerializeField] private InputActionReference inputAction;
    public InputAction Action { get { return inputAction; } }

    [SerializeField] private Vector3 destination;
    public Vector3 Destination { get { return destination; } }

    [SerializeField] private bool falling = false;
    public bool Falling { get { return falling; } }

    [SerializeField] private bool freezeTime = false;
    public bool FreezeTime { get { return freezeTime; } }

    [SerializeField] private int indexSkip = 0;
    public int IndexSkip { get { return indexSkip; } }
}
