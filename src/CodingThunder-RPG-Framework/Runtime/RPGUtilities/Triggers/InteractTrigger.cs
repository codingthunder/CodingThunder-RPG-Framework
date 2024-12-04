using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.GameState;
using CodingThunder.RPGUtilities.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder
{
    /// <summary>
    /// InteractTriggers will not trigger on their own. They need something else to act as the instigator.
    /// In many cases, that will be the Interactor.
    /// </summary>
    public class InteractTrigger : GameStateManaged
    {
        public bool interactable = false;
        /// <summary>
        /// Use to store any necessary variables, but most logic should be in Ink.
        /// </summary>
        public CmdSequence preInkSequence;
        [Header("Can also point to stitch.")]
        public string inkKnot;

        private bool sequenceRunning;

        public void RunTrigger()
        {
            if (!IsActive || sequenceRunning)
            {
                return;
            }

            if (preInkSequence == null)
            {
                StartInkKnot();
                return;
            }
            Debug.LogWarning("Running trigger.");

            sequenceRunning = true;
            StartCoroutine(preInkSequence.ExecuteCmdSequence(this, OnCompleteTriggerSequence, OnCancelTriggerSequence));
        }

        private void StartInkKnot()
        {
            GameRunner.Instance.StartStoryFlow(inkKnot);
        }

        private void OnCompleteTriggerSequence(CmdSequence sequence)
        {
            sequenceRunning = false;
            StartInkKnot();
        }

        private void OnCancelTriggerSequence(CmdSequence sequence)
        {
            sequenceRunning = false;
        }

        protected override void HandleGameStateChange(GameStateEnum state)
        {
            base.HandleGameStateChange(state);

            preInkSequence.SetIsSuspended(!IsActive);
        }
    }
}
