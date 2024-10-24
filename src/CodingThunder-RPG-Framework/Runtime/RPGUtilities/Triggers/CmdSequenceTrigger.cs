using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Triggers
{
	[RequireComponent(typeof(Collider2D))]
	public class CmdSequenceTrigger: GameStateManaged
	{
		[Header("Access the LookupID for the triggering GameObject with $$_ in your Cmds.")]
		public CmdSequence triggerSequence;

		private bool sequenceInProgress;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			Debug.Log("Trigger entered.");
			if (sequenceInProgress)
			{
				return;
			}
			if (!IsActive)
			{
				return;
			}

			sequenceInProgress = true;

			triggerSequence.localArgs["_"] = "$$Scene." + collision.name;

			Debug.Log("Beginning triggerSequence");
			StartCoroutine(triggerSequence.ExecuteCmdSequence(this, OnCompleteTriggerSequence));
		}

		private void OnCompleteTriggerSequence(CmdSequence sequence)
		{
			sequenceInProgress = false;
		}

        protected override void HandleGameStateChange(GameStateEnum state)
        {
            base.HandleGameStateChange(state);

			triggerSequence.SetIsSuspended(!IsActive);
        }
    }
}