using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAbility : MonoBehaviour
{
    private Vector3 previousPosition;
    public bool ghostAbilityAvailable = false;
    public int tilesMoved = 0;
    public int turnsLeft = 4;
    public bool GhostAbilityAvailable
    {
        get => ghostAbilityAvailable;
        set => ghostAbilityAvailable = value;
    }


    // Start is called before the first frame update

    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {  
        HasPlayerMoved();
        resetMovementCounter();
    }

    bool HasPlayerMoved(){
        if (transform.position != previousPosition)
        {
        // Debug.Log("Player has moved!");
            
            // Update the previous position
            previousPosition = transform.position;
            tilesMoved++;
            return true;
        }
        return false;
    }
    public void resetMovementCounter(){
        if(tilesMoved > turnsLeft ) tilesMoved = 0;
        return;
    }
}
