using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStats : MonoBehaviour
{
    [SerializeField] private float dash = 0.1f;
    public float Dash { get { return dash; } }
    [SerializeField] private float[] limit = { 30, 40, 50 };
    public float[] Limit { get { return limit; } }
    [SerializeField] private float[] power = {20, 30, 40};
    public float[] Power { get { return power; } }
    [SerializeField] private float cornering = 60;
    public float Cornering { get { return cornering; } }
    [SerializeField] private float[] boost = { 40, 46, 50 };    
    public float[] Boost { get { return boost; } }
    [SerializeField] private float airDepletion = 0.017f;
    public float AirDepletion { get { return airDepletion; } }
    [SerializeField] private float boostDepletion = 20;
    public float BoostDepletion { get { return boostDepletion; } }
    [SerializeField] private float boostTime = 5;
    public float BoostTime{ get { return boostTime; } }    
}
