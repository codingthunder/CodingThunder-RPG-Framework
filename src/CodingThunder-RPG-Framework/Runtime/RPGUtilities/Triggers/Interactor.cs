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
            Debug.Log("Interactor's OnTriggerEnter2D");
            if (collision.TryGetComponent<InteractTrigger>(out var component))
            {
                Debug.Log("Interactor's other component is an InteractTrigger.");
                if (!component.interactable)
                {
                    return;
                }
                Debug.Log("Found an interactable InteractTrigger.");
                receiver = component;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (receiver == null) { return; }
            if (collision.transform == receiver.transform)
            {
                Debug.Log("Removing receiver from interactor.");
                receiver = null;
            }
        }

        /// <summary>
        /// Whatever the Interactor is touching, make it do a flip.
        /// </summary>
        public void Interact()
        {
            Debug.Log("Attempting to interact!");
            if (!IsActive || receiver == null)
            {
                Debug.Log("Hey, there isn't a receiver on this interactor (or the interactor is inactive)");
                Debug.Log("Interactor Is Active: " + IsActive);
                return;
            }

            if (!receiver.interactable)
            {
                Debug.Log("Hey, the receiver in question isn't interactable.");
                return;
            }

            receiver.RunTrigger();
        }

    }
}
