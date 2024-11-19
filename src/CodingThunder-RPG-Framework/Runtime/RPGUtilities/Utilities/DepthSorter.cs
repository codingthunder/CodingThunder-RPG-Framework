using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CodingThunder.RPGUtilities.Utilities
{
    /// <summary>
    /// This isn't even DataManaged. It's just necessary.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class DepthSorter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private Vector2 previousPosition;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            RefreshSortingOrder();
        }

        private void Update()
        {
            if (previousPosition == (Vector2)transform.position)
            {
                return;
            }

            RefreshSortingOrder();
        }

        private void RefreshSortingOrder()
        {
            previousPosition = transform.position;
            int sortOrder = Mathf.RoundToInt(transform.position.y * -100);
            // Use a negative y position to ensure lower y values get higher sorting order
            spriteRenderer.sortingOrder = sortOrder;
            
        }
    }
}