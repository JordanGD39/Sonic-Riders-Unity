using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.GetComponent<TestHandleJoin>() == null)
        {
            List<GameObject> playersInScene = new List<GameObject>();
            playersInScene.AddRange(GameObject.FindGameObjectsWithTag(Constants.Tags.player));

            for (int i = 0; i < playersInScene.Count; i++)
            {
                playersInScene[i].SetActive(false);
            }

            GameManager.instance.GetComponent<PlayerConfigManager>().SpawnPlayers();
        }
    }
    
}
