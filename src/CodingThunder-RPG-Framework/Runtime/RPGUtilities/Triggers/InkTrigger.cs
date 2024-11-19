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
    public class InkTrigger : InteractTrigger
    {
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Hitting trigger.");
            RunTrigger(collision);
        }

    }
}
