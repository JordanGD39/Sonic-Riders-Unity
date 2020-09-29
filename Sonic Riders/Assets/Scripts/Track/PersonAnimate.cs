using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAnimate : MonoBehaviour
{
    public void DelayAnim()
    {
        Invoke("Animate", Random.Range(0, 5));
    }

    private void Animate()
    {
        GetComponent<Animator>().Play("Jump");
    }
}
