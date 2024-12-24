using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodingThunder.RPGUtilities.Behaviors
{
    public class RPGBehavior
    {
        public string BehaviorName { get; set; }
        //public Dictionary<string, Actor> Actors { get; set; }
        public BehaviorMetadata Metadata { get; set; }

        public CmdSequence StartSequence { get; set; }
        public CmdSequence EndSequence { get; set; }

        public CmdSequence TickSequence { get; set; }

        [BehaviorSequence]
        private void BuildBehaviorSequences(RPGBehavior behavior, string knot, Dictionary<string, Actor> actorsInvolved)
        {
            behavior.StartSequence = BehaviorBuilder.BuildSequenceFromStitch(knot + ".on_start", actorsInvolved);
            behavior.StartSequence = BehaviorBuilder.BuildSequenceFromStitch(knot + ".on_tick", actorsInvolved);
            behavior.StartSequence = BehaviorBuilder.BuildSequenceFromStitch(knot + ".on_finish", actorsInvolved);
        }

        [BehaviorMetadata]
        private void BuildBehaviorMetadata(RPGBehavior behavior, Dictionary<string, string> metadataArgs, Dictionary<string, Actor> actorKeywords)
        {
            // TODO: find a way to generalize this.
            // Right now, we're picking out the Actors that we KNOW the Behavior has jurisdiction over.
            // But that introduces more opportunities for fucking up.
            if (metadataArgs.TryGetValue("Source", out string sourceReference))
            {
                actorKeywords["Source"] = new RPGRef<Actor>() { ReferenceId = sourceReference };
                metadataArgs.Remove("Source");
            }

            if (metadataArgs.TryGetValue("Actor", out string actorReference))
            {
                actorKeywords["Actor"] = new RPGRef<Actor>() { ReferenceId = actorReference };
                metadataArgs.Remove("Actor");
            }

            BehaviorMetadata metadata = new BehaviorMetadata();

            metadata.Actor = actorKeywords["Actor"];
            metadata.Source = actorKeywords["Source"];

            var behaviorActors = new Dictionary<string, Actor>()
            {
                {"Actor", metadata.Actor },
                {"Source", metadata.Source }
            };

            

            ApplyMetadataArgs(metadataArgs, metadata, behaviorActors);

            behavior.Metadata = metadata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataArgs"></param>
        /// <param name="metadataObject">Can pass either the Behavior class itself or a referenced data class.</param>
        /// <param name="behaviorActors">Should be a subset of InvolvedActors that deal directly with the class.</param>
        /// <param name="metadataType">Must match the type of the data</param>
        protected static void ApplyMetadataArgs(Dictionary<string, string> metadataArgs, object metadataObject, Dictionary<string, Actor> behaviorActors)
        {
            var metadataProps = metadataObject.GetType()
                .GetProperties()
                .ToDictionary(x => x.Name, x => x);

            HashSet<string> usedArgs = new();

            foreach (var kv in metadataArgs)
            {
                var subbedValue = BehaviorBuilder.ReplaceActorKeywords(behaviorActors, kv.Value);

                //var subbedValue = kv.Value.Replace("$$Source", Actor.ToLookupString(metadata.Source)).Replace("$$Actor", Actor.ToLookupString(metadata.Actor));

                if (!metadataProps.ContainsKey(kv.Key))
                {
                    continue;
                }

                usedArgs.Add(kv.Key);
                var prop = metadataProps[kv.Key];

                //var prop = typeof(BehaviorMetadata).GetProperty(kv.Key);
                var value = LookupResolver.Instance.Resolve(subbedValue, prop.PropertyType);
                prop.SetValue(metadataObject, value);
            }

            foreach (var usedArg in usedArgs)
            {
                metadataArgs.Remove(usedArg);
            }
        }
    }
}