using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LadderObjectManager : MonoBehaviour
{
    private Vector3 positionLadder;
    private Vector3 positionPlayer;
    private Vector3 positionKey;
    private GameObject keyObject;

    [SerializeField]
    private Boolean active = false;

    // Start is called before the first frame update
    public void setLadder(Vector3 position)
    {
        positionLadder = position;
    }

    public void setKey(Vector3 position, GameObject key)
    {
        positionKey = position;
        keyObject = key;
    }

    public void unlockLadder()
    {
        active = true;
    }

    public void updatePlayerPosition(Vector3 playerPos)
    {
        positionPlayer = playerPos;
        checkPlayerOnLadder();
        checkPlayerOnKey();
    }

    private void checkPlayerOnLadder()
    {
        if (positionPlayer.x == positionLadder.x && positionPlayer.y == positionLadder.y)
        {
            if (active)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reloads scene
            }
        }
    }
    private void checkPlayerOnKey()
    {
        if (positionPlayer.x == positionKey.x && positionPlayer.y == positionKey.y)
        {
            active = true;
            
            keyObject.SetActive(false);
            unlockLadder();
            MapManager.instance.CreateEntity("EnabledLadder", positionLadder); // draw over old ladder sprite
        }
    }
}
