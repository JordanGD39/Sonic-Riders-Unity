using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private GameObject model;
    [SerializeField] private Transform questionMark;
    [SerializeField] private float respawnTime = 3;
    [SerializeField] private int ringCount = 10;
    [SerializeField] private float air = 15;
    [SerializeField] private float rotateSpeed = 60;

    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite[] itemSprites;
    private MeshRenderer meshRenderer;
    private int chosenIndex;

    private void Start()
    {
        source = GetComponentInChildren<AudioSource>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        //if (ringCount < 100)
        //{
            //hundredMark.SetActive(false);
            //RandomizeContents();
        //}
    }

    private void Update()
    {
        if (meshRenderer.isVisible)
            questionMark.Rotate(0, rotateSpeed * Time.deltaTime, 0);
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

        DetermineRandomItem(characterStats);

        /*characterStats.Rings += ringCount;
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
        }*/

        if (characterStats.IsPlayer && chosenIndex != 6)
            source.Play();
    }

    private void Respawn()
    {
        //RandomizeContents();
        model.SetActive(true);
    }

    private void DetermineRandomItem(CharacterStats player)
    {
        int placing = player.GetComponent<PlayerCheckpoints>().Place;
        
        //For 8th place are default

        //                   |  Rings  |   Air  | Spd | Invicibilty|
        //                    10 20 30  30 50 100                   %
        int[] percentages = { 0, 0, 30, 0, 0, 35, 25, 10};

        switch (placing)
        {
            case 0:
                percentages = new int[] { 50, 10, 0, 20, 11, 0, 8, 1};
                break;
            case 1:
                percentages = new int[] { 25, 25, 5, 15, 10, 0, 17, 3};
                break;
            case 2:
                percentages = new int[] { 6, 25, 10, 15, 15, 5, 18, 6};
                break;
            case 3:
                percentages = new int[] { 0, 17, 15, 15, 20, 5, 21, 7};
                break;
        }

        int prevPercent = 0;
        float rand = Random.Range(0, 100);

        for (int i = 0; i < percentages.Length; i++)
        {            
            prevPercent += percentages[i];

            Debug.Log("Percent: " + prevPercent + " Random number: " + rand + " Current percent: " + percentages[i]);

            if (rand <= prevPercent)
            {
                chosenIndex = i;
                break;
            }
        }

        //Used to test certain items
        chosenIndex = 6;

        int amount = 0;
        int itemIndex = 0;

        switch (chosenIndex)
        {
            case 0:
                player.Rings += 10;
                amount = 10;
                break;
            case 1:
                player.Rings += 20;
                amount = 20;
                break;
            case 2:
                player.Rings += 30;
                amount = 30;
                break;
            case 3:
                if (!player.BoardStats.RingsAsAir)
                {
                    player.Air += 30;
                    amount = 30;
                    itemIndex = 1;
                }
                else
                {
                    player.Rings += 15;
                    amount = 15;
                }                
                
                break;
            case 4:
                if (!player.BoardStats.RingsAsAir)
                {
                    player.Air += 50;
                    amount = 50;
                    itemIndex = 1;
                }
                else
                {
                    player.Rings += 15;
                    amount = 15;
                }

                break;
            case 5:
                if (!player.BoardStats.RingsAsAir)
                {
                    player.Air += 100;
                    amount = 100;
                    itemIndex = 1;
                }
                else
                {
                    player.Rings += 30;
                    amount = 30;
                }

                break;
            case 6:
                player.SpeedShoesCountDown();
                itemIndex = 2;
                break;
            case 7:
                player.Invincibility();
                itemIndex = 3;
                break;
        }

        player.Hud.ShowItem(itemIndex, amount);
    }

    //private void RandomizeContents()
    //{
    //    int item = Random.Range(0, 4);       

    //    ringCount = 0;
    //    air = 0;

    //    /*switch (item)
    //    {
    //        case 0:
    //            ringCount = randAmount * 10;
    //            itemToChange.sprite = itemSprites[item];
    //            break;
    //        case 1:
    //            air = randAmount * 10;
    //            itemToChange.sprite = itemSprites[item];
    //            break;
    //    }*/

    //    if (item < 1)
    //    {
    //        int randAmount = Random.Range(1, 3);
    //        //numberToChange.sprite = numbers[randAmount - 1];

    //        ringCount = randAmount * 10;
    //        //itemToChange.sprite = itemSprites[0];
    //    }
    //    else
    //    {
    //        int randAmount = Random.Range(1, 6);
    //       // numberToChange.sprite = numbers[randAmount - 1];
    //        air = randAmount * 10;
    //        //itemToChange.sprite = itemSprites[1];
    //    }
    //}
}
