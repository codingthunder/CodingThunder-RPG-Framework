using CodingThunder.RPGUtilities.DataManagement;
using System.Collections.Generic;
using UnityEngine.Profiling.Memory.Experimental;

namespace CodingThunder.RPGUtilities.Behaviors
{
    public class Effect: RPGBehavior
    {
        public EffectData EffectData { get; set; }

        [BehaviorMetadata]
        public static void BuildEffectMetadata(RPGBehavior behavior, Dictionary<string, string> metadataArgs, Dictionary<string, Actor> actorKeywords)
        {
            Effect effect = (Effect)behavior;

            //effect.EffectData = new();

            if (metadataArgs.TryGetValue("Target", out string targetReference))
            {
                actorKeywords["Target"] = new RPGRef<Actor>() { ReferenceId = targetReference };
                metadataArgs.Remove("Target");
            }

            var effectData = new EffectData();

            effectData.Target = actorKeywords["Target"];

            var effectActors = new Dictionary<string, Actor>()
            {
                {"Target", effectData.Target }
            };

            ApplyMetadataArgs(metadataArgs, effectData, effectActors);

            effect.EffectData = effectData;
        }

        // Should be unnecessary. Probably.
        //public static void BuildEffectSequences(RPGBehavior behavior, string knot, Dictionary<string, Actor> actorsInvolved)
        //{

        //}
    }
}