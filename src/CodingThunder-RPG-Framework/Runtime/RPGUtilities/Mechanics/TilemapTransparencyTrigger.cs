using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodingThunder.RPGUtilities.Mechanics
{
    [RequireComponent(typeof(TilemapRenderer))]
    public class TilemapTransparencyTrigger : MonoBehaviour
    {
        private TilemapRenderer tilemapRenderer;
        private Color originalColor;

        [Range(0f, 1f)]
        public float opacityOnEnter = 0.5f;  // Set this value to control how transparent the tilemap becomes

        private void Start()
        {
            // Get the TilemapRenderer component
            tilemapRenderer = GetComponent<TilemapRenderer>();

            // Store the original color (including original opacity)
            originalColor = tilemapRenderer.material.color;
        }

        // When the player enters the collider trigger
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))  // Assuming the player has the tag "Player"
            {
                SetTilemapOpacity(opacityOnEnter);
            }
        }

        // When the player exits the collider trigger
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetTilemapOpacity(1f);  // Restore full opacity
            }
        }

        // Helper function to set the tilemap opacity
        private void SetTilemapOpacity(float opacity)
        {
            // Create a new color with the same RGB values but a different alpha (opacity)
            Color newColor = originalColor;
            newColor.a = opacity;

            // Apply the new color to the material's color property
            tilemapRenderer.material.color = newColor;
        }
    }
}