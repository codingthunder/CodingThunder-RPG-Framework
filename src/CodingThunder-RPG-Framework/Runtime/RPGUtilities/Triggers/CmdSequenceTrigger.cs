using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Triggers
{
	/// <summary>
	/// Should be deferred to InkTrigger. Retaining for edge cases,
	/// but this component is an unwieldy pain in the ass to work with.
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class CmdSequenceTrigger: GameStateManaged
	{
		[Header("Sequences are processed one after the other." +
			"\nIf one is skipped, the next will still run if its condition is valid.")]
		public List<CmdSequence> triggerSequences;

		////Because
		//private bool sequenceInProgress;

		//Starts at -1 because that means NO sequence is running.
		private int activeIndex = -1;
		private Collider2D otherCollider;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (activeIndex > -1)
			{
				return;
			}
			if (!IsActive)
			{
				return;
			}

			otherCollider = collision;

			RunNextSequence();

		}

		private void RunNextSequence()
		{
			activeIndex++;

			var triggerSequence = triggerSequences[activeIndex];

            triggerSequence.localArgs["_"] = "$$Scene." + otherCollider.name;

			//Either way, it needs to move to the next sequence even if one doesn't pass.
            StartCoroutine(triggerSequence.ExecuteCmdSequence(this, OnCompleteTriggerSequence, OnCompleteTriggerSequence));
        }

		private void OnCompleteTriggerSequence(CmdSequence sequence)
		{
			if (activeIndex >= triggerSequences.Count - 1)
			{
				activeIndex = -1;
				otherCollider = null;
				return;
			}

			RunNextSequence();
		}


        protected override void HandleGameStateChange(GameStateEnum state)
        {
            base.HandleGameStateChange(state);

			foreach (var triggerSequence in triggerSequences)
			{
				triggerSequence.SetIsSuspended(!IsActive);
			}
        }
    }
}