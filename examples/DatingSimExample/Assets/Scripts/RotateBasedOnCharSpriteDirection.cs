
using UnityEngine;

public class RotateBasedOnCharSpriteDirection : MonoBehaviour
{
    public CharSpriteDirection spriteDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = spriteDirection.CurrentSpriteDirection;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the z-axis
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
