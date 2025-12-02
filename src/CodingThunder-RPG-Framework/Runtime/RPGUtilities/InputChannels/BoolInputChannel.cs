using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.InputChannels
{
    [CreateAssetMenu(menuName = "Input Channels/Bool")]
    public class BoolInputChannel : ScriptableObject
    {
        public event Action<bool> OnRaised;

        // Optional: keep the latest value so listeners can poll on spawn
        [NonSerialized] public bool LastValue;

        public void Raise(bool value)
        {
            LastValue = value;
            OnRaised?.Invoke(value);
        }
        
        // Add this so the UnityEvent can see a matching signature
        public void RaiseFromContext(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            Raise(ctx.ReadValue<bool>());
        }
        
        // This is for Press & Release
        public void RaiseFromPressAndRelease(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                Raise(true);
                return;
            }

            if (ctx.canceled)
            {
                Raise(false);
                return;
            }
        }
    }
}