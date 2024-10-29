using CodingThunder.RPGUtilities.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not extending GameStateManaged. I see no reason why this would ever NOT work.
/// </summary>
public class CharSpriteDirection : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Movement2D movement;

    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private Vector2 currentMoveDirection = Vector2.zero;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (movement == null)
        {
            movement = GetComponentInChildren<Movement2D>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //This is a reaaaally hackish way to do this. The correct way would be to set up some sort of callback.
        //But ask me if I care right now.
        var updatedDir = movement.m_direction;

        if (updatedDir.normalized == currentMoveDirection.normalized )
        {
            return;
        }

        currentMoveDirection = updatedDir;

        var snappedDir = SnapToCardinalDirection(SnapToCardinalDirection(updatedDir));

        //Process of elimination. If it's not up, left, or right, it's down.
        var currentSprite = downSprite;
        if (snappedDir == Vector2.left )
        {
            currentSprite = leftSprite;
        }
        if (snappedDir == Vector2.right )
        {
            currentSprite = rightSprite;
        }
        if (snappedDir == Vector2.up )
        {
            currentSprite = upSprite;
        }

        spriteRenderer.sprite = currentSprite;
    }

    public static Vector2 SnapToCardinalDirection(Vector2 vector)
    {
        // Define the cardinal directions
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };

        // Initialize variables to track the closest direction
        Vector2 closestDirection = Vector2.zero;
        float maxDot = -Mathf.Infinity;

        // Check each direction to see which one is closest
        foreach (var direction in directions)
        {
            float dot = Vector2.Dot(vector.normalized, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestDirection = direction;
            }
        }

        return closestDirection;
    }
}
