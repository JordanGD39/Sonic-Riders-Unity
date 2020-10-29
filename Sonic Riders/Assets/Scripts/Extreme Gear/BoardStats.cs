using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Board", menuName = "Board")]
public class BoardStats : ScriptableObject
{
    [SerializeField] private string boardName = "";
    public string BoardName { get { return boardName; } }
    [SerializeField] private Sprite boardIcon;
    public Sprite BoardIcon { get { return boardIcon; } }
    [SerializeField] private float dash = 10f;
    public float Dash { get { return dash; } }
    [SerializeField] private float limit = 54;
    public float Limit { get { return limit; } }
    [SerializeField] private float[] power = { 21.5f, 23.75f, 23.75f };
    public float[] Power { get { return power; } }
    [SerializeField] private float cornering = 60;
    public float Cornering { get { return cornering; } }
    [SerializeField] private float[] boost = { 50, 57.5f, 62.5f };
    public float[] Boost { get { return boost; } }
    [SerializeField] private int[] maxAir = { 100, 150, 200 };
    public int[] MaxAir { get { return maxAir; } }
    [SerializeField] private float airDepletion = 0.017f;
    public float AirDepletion { get { return airDepletion; } }
    [SerializeField] private float[] boostDepletion = {25, 30, 40 };
    public float[] BoostDepletion { get { return boostDepletion; } }
    [SerializeField] private float[] driftDepletion = {166, 250, 333 };
    public float[] DriftDepletion { get { return driftDepletion; } }
    [SerializeField] private float[] tornadoCost = {25, 30, 40 };
    public float[] TornadoCost { get { return tornadoCost; } }    
    [SerializeField] private type boardType;
    public type BoardType { get { return boardType; } }
    [SerializeField] private bool standard = false;
    public bool IsStandard { get { return standard; } }
    [SerializeField] private bool changeType = false;
    public bool ChangeType { get { return changeType; } }
    [SerializeField] private GameObject[] characterRestriction;
    public GameObject[] CharacterRestriction { get { return characterRestriction; } }
}
