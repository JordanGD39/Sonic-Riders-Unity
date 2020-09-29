using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceSpawn : MonoBehaviour
{
    [SerializeField] private Sprite[] people;
    [SerializeField] private GameObject personPref;

    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = transform.GetChild(0);

        for (int i = 0; i < spawn.childCount; i++)
        {
            Transform place = spawn.GetChild(i);

            for (int j = 0; j < 8; j++)
            {
                GameObject person = Instantiate(personPref, place, false);

                float z = -40;
                z += j * 10;
                //Debug.Log(z);
                person.transform.localPosition = new Vector3(Random.Range(-1, 1), 0, z + Random.Range(-2.5f, 2.5f));
                person.GetComponentInChildren<SpriteRenderer>().sprite = people[Random.Range(0, people.Length - 1)];
                person.GetComponentInChildren<PersonAnimate>().DelayAnim();
            }            
        }
    }
}
