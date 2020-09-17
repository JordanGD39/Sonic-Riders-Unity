using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class EventSystemHolder : MonoBehaviour
{
    private List<MultiplayerEventSystem> multiplayerEventSystems = new List<MultiplayerEventSystem>();
    public List<MultiplayerEventSystem> MultiplayerEventSystems { get { return multiplayerEventSystems; } }
}
