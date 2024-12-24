using CodingThunder.RPGUtilities.Cmds;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Behaviors
{
    public interface IRPGBehavior
    {
        public CmdSequence StartSequence { get; set; }
        public CmdSequence EndSequence { get; set; }

        public CmdSequence TickSequence { get; set; }

        public List<GameObject> SourceChain { get; set; }
    }
}