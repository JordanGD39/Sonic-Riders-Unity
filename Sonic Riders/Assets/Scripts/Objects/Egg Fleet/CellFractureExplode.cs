using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFractureExplode : MonoBehaviour
{
    [SerializeField] private GameObject cellsParent;
    public Transform CellsParent { get { return cellsParent.transform; } }
    [SerializeField] private GameObject nonBrokenModel;
    public GameObject NonBrokenModel { get { return nonBrokenModel; } }
    public delegate void DestroyedFuntion();
    public DestroyedFuntion Destroyed;

    // Start is called before the first frame update
    void Start()
    {
        nonBrokenModel.SetActive(true);
        cellsParent.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody != null && collision.collider.attachedRigidbody.gameObject.CompareTag(Constants.Tags.player))
        {
            CharacterStats characterStats = collision.collider.attachedRigidbody.GetComponent<CharacterStats>();

            if (!characterStats.TypeCheck(type.POWER) || characterStats.Air <= 0){ return; }

            nonBrokenModel.SetActive(false);
            cellsParent.SetActive(true);

            Destroyed();
        }
    }
}
