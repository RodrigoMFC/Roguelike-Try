using UnityEngine;

/// <summary>
/// A generic class to represent players, enemies, items, etc.
/// </summary>
public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;
    public int movementCost = 0;
    int movementIter = 0;
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }

    public void AddToGameManager()
    {
        GameManager.instance.Entities.Add(this);
    }

    public void Move(Vector2 direction)
    {
        if (movementIter < movementCost)
        {
            movementIter++;
            return;
        }
        movementIter = 0;
        transform.position += (Vector3)direction;
    }
}