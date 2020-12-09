using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggPawnWalkList : MonoBehaviour
{
    [SerializeField] private List<Collider> colliderBounds = new List<Collider>();
    public List<Collider> ColliderBoundsList { get { return colliderBounds; } }
}
