namespace CodingThunder.RPGUtilities.TurnBasedBattle
{
    public class BattleAction
    {
        public string Name { get; }
        public int Cost { get; }
        public string Description { get; }

        public BattleAction(string name, int cost, string description)
        {
            Name = name;
            Cost = cost;
            Description = description;
        }
    }
}
