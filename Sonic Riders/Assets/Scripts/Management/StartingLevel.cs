﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingLevel : MonoBehaviour
{
    private Transform psParent;
    private List<Text> countdownTexts = new List<Text>();

    [SerializeField] private bool noStart = false;

    private bool startCountDown = false;
    private float timer = 5;
    public float Timer { get { return timer; } }

    private bool doneCounting = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.GetComponent<TestHandleJoin>() == null)
        {
            List<GameObject> playersInScene = new List<GameObject>();
            playersInScene.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

            for (int i = 0; i < playersInScene.Count; i++)
            {
                Destroy(playersInScene[i]);
            }

            GameManager.instance.GetComponent<PlayerConfigManager>().SpawnPlayers(this);
        }

        psParent = GetComponentInChildren<ParticleSystem>().transform.parent;

        List<GameObject> texts = new List<GameObject>();
        texts.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.countdown));

        for (int i = 0; i < texts.Count; i++)
        {
            countdownTexts.Add(texts[i].GetComponent<Text>());
        }
    }

    private void Update()
    {
        if (!startCountDown)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < -1)
        {
            psParent.gameObject.SetActive(false);
            enabled = false;
        }

        if (doneCounting)
        {
            return;
        }

        for (int i = 0; i < countdownTexts.Count; i++)
        {
            countdownTexts[i].text = timer.ToString("F2");
        }

        if (timer <= 0)
        {
            doneCounting = true;

            for (int i = 0; i < psParent.childCount; i++)
            {
                psParent.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }

            for (int i = 0; i < countdownTexts.Count; i++)
            {
                countdownTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlacePlayersInOrder()
    {
        if (noStart)
        {
            for (int i = 0; i < countdownTexts.Count; i++)
            {
                countdownTexts[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < psParent.childCount; i++)
            {
                psParent.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }

            psParent.gameObject.SetActive(false);

            timer = -1;

            return;
        }

        List<GameObject> playersInScene = new List<GameObject>();
        playersInScene.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

        for (int i = 0; i < playersInScene.Count; i++)
        {
            if (i > 4)
            {
                i = 4 - i;
            }

            float x = i * 3;

            playersInScene[i].transform.position = new Vector3(x, 0.4f, 0);
        }

        startCountDown = true;
    }
}
