using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Board", menuName = "Board")]
public class BoardStats : ScriptableObject
{
    [SerializeField] private float dash = 10f;
    public float Dash { get { return dash; } }
    [SerializeField] private float[] limit = { 40.5f, 42.75f, 45 };
    public float[] Limit { get { return limit; } }
    [SerializeField] private float[] power = { 21.5f, 23.75f, 23.75f };
    public float[] Power { get { return power; } }
    [SerializeField] private float cornering = 60;
    public float Cornering { get { return cornering; } }
    [SerializeField] private float[] boost = { 50, 57.5f, 62.5f };
    public float[] Boost { get { return boost; } }
    [SerializeField] private float airDepletion = 0.017f;
    public float AirDepletion { get { return airDepletion; } }
    [SerializeField] private float boostDepletion = 15;
    public float BoostDepletion { get { return boostDepletion; } }
    [SerializeField] private float boostTime = 5;
    public float BoostTime { get { return boostTime; } }
    [SerializeField] private type boardType;
    public type BoardType { get { return boardType; } }
    [SerializeField] private bool standard = false;
    public bool IsStandard { get { return standard; } }
}
