using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Utilities
{
    /// <summary>
    /// Works alongside with DepthSorter. Runs on LateUpdate so that the other Renderer always gets its sortingOrder
    /// updated first.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class DepthOffsetMatcher : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public SpriteRenderer otherRenderer;

        private int offset;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (otherRenderer == null)
            {
                Debug.LogError("Don't forget to assign the renderer your matching on your OrderMatcher." +
                    " Check GameObject" + gameObject.name);
            }

            offset = spriteRenderer.sortingOrder - otherRenderer.sortingOrder;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void LateUpdate()
        {
            if (otherRenderer.sortingOrder + offset != spriteRenderer.sortingOrder)
            {
                spriteRenderer.sortingOrder = otherRenderer.sortingOrder + offset;
            }
            
        }
    }
}
