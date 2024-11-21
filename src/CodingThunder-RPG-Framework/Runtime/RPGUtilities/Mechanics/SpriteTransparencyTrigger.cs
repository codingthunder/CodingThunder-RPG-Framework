using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodingThunder.RPGUtilities.Mechanics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteTransparencyTrigger : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        [Range(0f, 1f)]
        public float opacityOnEnter = 0.5f;  // Set this value to control how transparent the tilemap becomes

        private void Start()
        {
            // Get the SpriteRenderer component
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Store the original color (including original opacity)
            originalColor = spriteRenderer.material.color;
        }

        // When the player enters the collider trigger
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))  // Assuming the player has the tag "Player"
            {
                SetSpriteOpacity(opacityOnEnter);
            }
        }

        // When the player exits the collider trigger
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetSpriteOpacity(1f);  // Restore full opacity
            }
        }

        // Helper function to set the tilemap opacity
        private void SetSpriteOpacity(float opacity)
        {
            // Create a new color with the same RGB values but a different alpha (opacity)
            Color newColor = originalColor;
            newColor.a = opacity;

            // Apply the new color to the material's color property
            spriteRenderer.material.color = newColor;
        }
    }
}