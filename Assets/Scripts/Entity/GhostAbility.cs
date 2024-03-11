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

    //neste if meter que quando e preciso fazer reset os inimigos tem que se mover primeiro ent turn end 
    public void resetMovementCounter(){
        if(tilesMoved > turnsLeft ) tilesMoved = 0;
        return;
    }

    // space is pressed
    // player turn speed = 4 
    // enemy turn speed = 1

    public void useGhostAbility()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
                ghostAbilityAvailable = !ghostAbilityAvailable;
                ghost.transform.position = transform.position;
                Debug.Log("REACHED!");
                ghost.SetActive(ghostAbilityAvailable);            
        }
    }
}