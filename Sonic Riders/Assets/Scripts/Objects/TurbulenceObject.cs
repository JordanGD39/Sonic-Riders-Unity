using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbulenceObject : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        Material mat = new Material(meshRenderers[0].material);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = mat;
        }
    }

    public void FadeOut()
    {
        anim.SetTrigger("FadeOut");
    }

    public void SetUnactive()
    {
        gameObject.SetActive(false);
    }
}
