using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactor : GameStateManaged
    {
        private InteractTrigger receiver = null;
        
        /// <summary>
        /// Should run even if Interactor isn't active. Otherwise, could get in funky state.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<InteractTrigger>(out var component))
            {
                if (!component.interactable)
                {
                    return;
                }
                receiver = component;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (receiver == null) { return; }
            if (collision.transform == receiver.transform)
            {
                receiver = null;
            }
        }

        /// <summary>
        /// Whatever the Interactor is touching, make it do a flip.
        /// </summary>
        public void Interact()
        {
            if (!IsActive || receiver == null)
            {
                return;
            }

            if (!receiver.interactable)
            {
                return;
            }

            receiver.RunTrigger();
        }

    }
}
