using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;

namespace CodingThunder.RPGUtilities.Behaviors
{

    public class Actor: GameStateManaged
    {


        public static string ToLookupString(object actorObject)
        {
            Actor actor = (Actor)actorObject;



            return "$$Scene." + actor.gameObject.name + $".GetComponent(\"{actor.GetType().Name}\")";
        }
    }

}