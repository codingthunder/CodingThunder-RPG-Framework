using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Triggers
{
    /// <summary>
    /// Prefer this over CmdSequenceTrigger. At present, it is much easier to write Cmds
    /// in Ink than to use that abominable UI system. These triggers are necessarily blocking.
    /// To have "events" happen in parallel with the gameplay, trigger CmdSequences
    /// from within Ink, and set their ink tag to #auto.
    /// </summary>
    /// 
    [RequireComponent(typeof(Collider2D))]
    public class InkTrigger : GameStateManaged
    {
        /// <summary>
        /// Use to store any necessary variables, but most logic should be in Ink.
        /// </summary>
        public CmdSequence preInkSequence;
        [Header("Can also point to stitch.")]
        public string inkKnot;

        private bool sequenceRunning;

        private void OnTriggerEnter2D(Collider2D collision)
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

            sequenceRunning = true;
            StartCoroutine(preInkSequence.ExecuteCmdSequence(this, OnCompleteTriggerSequence,OnCancelTriggerSequence));
        }

        private void StartInkKnot()
        {
            GameRunner.Instance.StartCutscene(inkKnot);
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
