using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.InputChannels
{
    [CreateAssetMenu(menuName = "Input Channels/Vector2")]
    public class Vector2InputChannel : ScriptableObject
    {
        public event Action<Vector2> OnRaised;

        // Optional: keep the latest value so listeners can poll on spawn
        [NonSerialized] public Vector2 LastValue;

        public void Raise(Vector2 value)
        {
            LastValue = value;
            OnRaised?.Invoke(value);
        }
        
        // Add this so the UnityEvent can see a matching signature
        public void RaiseFromContext(InputAction.CallbackContext ctx)
        {
            // Usually you only care on performed/canceled, but up to you:
            if (ctx.started) return;
            Raise(ctx.ReadValue<Vector2>());
        }
    }
}