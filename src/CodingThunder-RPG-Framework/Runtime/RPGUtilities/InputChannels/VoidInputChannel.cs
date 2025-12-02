using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.InputChannels
{
    [CreateAssetMenu(menuName = "Input Channels/Void")]
    public class VoidInputChannel : ScriptableObject
    {
        public event Action OnRaised;

        public void Raise() => OnRaised?.Invoke();


        // Add this so the UnityEvent can see a matching signature
        public void RaiseFromContext(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            Raise();
        }
    }
}