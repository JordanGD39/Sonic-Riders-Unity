using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlashes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("CameraFlash");
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator CameraFlash()
    {
        yield return new WaitForSeconds(Random.Range(0, 3));

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 1));
            Transform flash = transform.GetChild(0);
            flash.localPosition = new Vector3(Random.Range(-30, 30), 0, Random.Range(-18, 18));

            flash.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            flash.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            flash.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            flash.gameObject.SetActive(false);
        }        
    }
}
