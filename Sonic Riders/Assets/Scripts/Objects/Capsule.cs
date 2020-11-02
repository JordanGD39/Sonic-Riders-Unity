using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    //Almost same as ring later will be a lot different

    private AudioSource source;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject hundredMark;
    [SerializeField] private SpriteRenderer numberToChange;
    [SerializeField] private SpriteRenderer itemToChange;
    [SerializeField] private float respawnTime = 3;
    [SerializeField] private int ringCount = 10;
    [SerializeField] private float air = 15;

    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite[] itemSprites;

    private void Start()
    {
        source = GetComponentInChildren<AudioSource>();

        if (ringCount < 100)
        {
            hundredMark.SetActive(false);
            RandomizeContents();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 8)
        {
            return;
        }

        model.SetActive(false);
        Invoke("Respawn", respawnTime);
        CharacterStats characterStats = other.GetComponentInParent<CharacterStats>();
        characterStats.Rings += ringCount;
        if (!characterStats.BoardStats.RingsAsAir)
        {
            characterStats.Air += air;
        }
        else
        {
            if (air > 0)
            {
                characterStats.Rings += 5;
            }
        }

        if(characterStats.IsPlayer)
            source.Play();
    }

    private void Respawn()
    {
        RandomizeContents();
        model.SetActive(true);
    }

    private void RandomizeContents()
    {
        int item = Random.Range(0, 4);       

        ringCount = 0;
        air = 0;

        /*switch (item)
        {
            case 0:
                ringCount = randAmount * 10;
                itemToChange.sprite = itemSprites[item];
                break;
            case 1:
                air = randAmount * 10;
                itemToChange.sprite = itemSprites[item];
                break;
        }*/

        if (item < 1)
        {
            int randAmount = Random.Range(1, 3);
            numberToChange.sprite = numbers[randAmount - 1];

            ringCount = randAmount * 10;
            itemToChange.sprite = itemSprites[0];
        }
        else
        {
            int randAmount = Random.Range(1, 6);
            numberToChange.sprite = numbers[randAmount - 1];
            air = randAmount * 10;
            itemToChange.sprite = itemSprites[1];
        }
    }
}
