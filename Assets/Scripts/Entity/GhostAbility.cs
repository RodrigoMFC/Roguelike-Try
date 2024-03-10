using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAbility : MonoBehaviour
{
    private Vector3 previousPosition;
   [SerializeField] private bool ghostAbilityAvailable = false;
    [SerializeField] private int tilesMoved = 0;
    [SerializeField] private int turnsLeft = 4;
    [SerializeField] private GameObject ghost;
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
        useGhostAbility();
       
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

    // space is pressed
    // player turn speed = 4 
    // enemy turn speed = 1
    public void useGhostAbility(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ghostAbilityAvailable = !ghostAbilityAvailable;
            Instantiate(ghost, new Vector3(/*posicao player*/), Quaternion.identity).name = "Ghost";
        }
            if(ghostAbilityAvailable){

        }
    }
}

