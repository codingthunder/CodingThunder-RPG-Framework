using System.Collections.Generic;
using System.Linq;

namespace CodingThunder.RPGUtilities.TurnBasedBattle
{
    public class Combatant
    {
        public string Name { get; set; }
        public bool IsPlayer { get; set; }
        public int HP { get; set; }
        public int Agility { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Soul { get; set; }
        public int Team { get; set; }

        private List<BattleAction> _battleActions = new List<BattleAction>();

        public IEnumerable<BattleAction> BattleActions
        {
            get
            {
                return _battleActions
                    .OrderBy(action => action.Cost)
                    .ThenBy(action => action.Name);
            }
        }

        public void AddBattleAction(BattleAction action)
        {
            if (action != null)
            {
                _battleActions.Add(action);
            }
        }
    }
}