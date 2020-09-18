using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.GetComponent<PlayerConfigManager>().SpawnPlayers();
    }
    
}
