using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReassembleCellParts : MonoBehaviour
{
    private List<Vector3> cellPositions = new List<Vector3>();
    private List<Quaternion> cellRotations = new List<Quaternion>();
    private Transform cellsParent;
    private GameObject nonBrokenModel;
    [SerializeField] private float destroyTime = 5;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        CellFractureExplode cellFractureExplode = GetComponent<CellFractureExplode>();
        cellFractureExplode.Destroyed += WaitForAssembly;
        cellsParent = cellFractureExplode.CellsParent;
        nonBrokenModel = cellFractureExplode.NonBrokenModel;
        animator = GetComponent<Animator>();

        for (int i = 0; i < cellsParent.childCount; i++)
        {
            Transform cell = cellsParent.GetChild(i);
            cellPositions.Add(cell.position);
            cellRotations.Add(cell.rotation);
        }
    }

    private void WaitForAssembly()
    {
        Invoke("Reassemble", destroyTime);
    }

    private void Reassemble()
    {
        cellsParent.gameObject.SetActive(false);

        for (int i = 0; i < cellsParent.childCount; i++)
        {
            Transform cell = cellsParent.GetChild(i);

            cell.position = cellPositions[i];
            cell.rotation = cellRotations[i];
        }

        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }

        nonBrokenModel.SetActive(true);
    }
}
