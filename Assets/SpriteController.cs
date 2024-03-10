using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites; // Array to hold all sprites for different directions

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Method to set the sprite based on the direction
    public void SetSprite(Vector2 direction)
    {
        // Calculate the angle of the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Determine the direction based on the angle
        Direction directionEnum = Direction.Right; // Default direction
        if (angle > -45 && angle <= 45)
            directionEnum = Direction.Right;
        else if (angle > 45 && angle <= 135)
            directionEnum = Direction.Up;
        else if (angle > 135 || angle <= -135)
            directionEnum = Direction.Left;
        else if (angle > -135 && angle <= -45)
            directionEnum = Direction.Down;

        // Set the sprite based on the direction
        SetSprite(directionEnum);
    }

    // Method to set the sprite based on the direction
    public void SetSprite(Direction direction)
    {
        // Assuming you have sprites ordered in the array as follows: Up, Down, Left, Right
        // You can adjust the ordering as needed based on your sprite sheet layout
        switch (direction)
        {
            case Direction.Right:
                spriteRenderer.sprite = sprites[0];
                break;
            case Direction.Up:
                spriteRenderer.sprite = sprites[1];
                break;
            case Direction.Left:
                spriteRenderer.sprite = sprites[2];
                break;
            case Direction.Down:
                spriteRenderer.sprite = sprites[3];
                break;
            default:
                break;
        }
    }
}

// Enumeration to represent different directions
public enum Direction
{
    Right,
    Up,
    Left,
    Down
}