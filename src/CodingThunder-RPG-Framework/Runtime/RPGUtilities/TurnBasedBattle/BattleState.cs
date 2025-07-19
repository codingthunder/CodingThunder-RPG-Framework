using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.TurnBasedBattle
{
    public class BattleState
    {

        public Dictionary<string, Combatant> Combatants { get; private set; } = new Dictionary<string, Combatant>();
        public int Round { get; set; }
        public string Turn { get; set; }

        public bool WaitingForAction { get; set; } = false;
        public string SelectedAction { get; set; }
        public string SelectedTarget { get; set; }
    }
}
