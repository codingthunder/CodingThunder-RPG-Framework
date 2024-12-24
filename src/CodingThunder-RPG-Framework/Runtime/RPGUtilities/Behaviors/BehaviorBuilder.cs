using UnityEngine;
using System.Collections.Generic;
using Ink.Runtime;
using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.DataManagement;
using System.Linq;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodingThunder.RPGUtilities.Behaviors
{
    /// <summary>
    /// Should only be placed over Static methods that match the BehaviorSequence signature of
    /// Action<RPGBehavior, string, Dictionary<string, Actor>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BehaviorSequenceAttribute : Attribute
    {
    }

    /// <summary>
    /// Should only be placed over Static methods that match the BehaviorMetadata signature of
    /// Action<RPGBehavior, Dictionary<string, string>, Dictionary<string, Actor>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BehaviorMetadataAttribute : Attribute
    {
    }

    public static class BehaviorBuilder
    {


        private static Dictionary<Type, Action<RPGBehavior, Dictionary<string, string>, Dictionary<string, Actor>>> buildMetadataCallbacks = new();
        private static Dictionary<Type, Action<RPGBehavior, string, Dictionary<string, Actor>>> buildSequenceCallbacks = new();

        private static Dictionary<string, Type> behaviorTypeLookup;

        private static void RegisterBehaviorMetadataBuilder(Type behaviorType)
        {
            RegisterCallback(
                behaviorType,
                typeof(BehaviorMetadataAttribute),
                buildMetadataCallbacks
            );
        }

        private static void RegisterBuildSequenceCallback(Type behaviorType)
        {
            RegisterCallback(
                behaviorType,
                typeof(BehaviorSequenceAttribute),
                buildSequenceCallbacks
            );
        }



        private static void RegisterCallback<TDelegate>(
    Type behaviorType,
    Type attributeType,
    Dictionary<Type, TDelegate> callbackDictionary)
    where TDelegate : Delegate
        {
            if (!behaviorType.IsSubclassOf(typeof(RPGBehavior)) && behaviorType != typeof(RPGBehavior))
            {
                throw new ArgumentException("When registering callbacks, please be sure your type inherits from RPGBehavior.");
            }

            try
            {
                var methods = behaviorType
                    .GetMethods()
                    .Where(m => m.GetCustomAttribute(attributeType) != null)
                    .ToList();

                foreach (var method in methods)
                {
                    var action = (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);

                    if (!callbackDictionary.ContainsKey(behaviorType))
                    {
                        callbackDictionary.Add(behaviorType, action);
                        continue;
                    }

                    callbackDictionary[behaviorType] = (TDelegate)Delegate.Combine(callbackDictionary[behaviorType], action);
                }
            }
            catch (Exception e)
            {
                // TODO: Handle exception
                throw e;
            }
        }



        /// <summary>
        /// Idempotent. If behaviorTypeLookup isn't null, won't run.
        /// </summary>
        public static void InitializeBehaviorTypeLookup()
        {
            if (behaviorTypeLookup != null)
            {
                return;
            }

            behaviorTypeLookup = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                var behaviorTypes = types.Where(type => typeof(RPGBehavior).IsAssignableFrom(type) && !type.IsAbstract);

                foreach (Type type in behaviorTypes)
                {
                    if (!behaviorTypeLookup.ContainsKey(type.Name))
                    {
                        behaviorTypeLookup.Add(type.Name, type);
                        RegisterBehaviorMetadataBuilder(type);
                        RegisterBuildSequenceCallback(type);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate Behavior type name found: {type.Name}. Skipping.");
                    }
                }
            }
        }



        public static RPGBehavior BuildBehavior(string knot, Dictionary<string, Actor> actorsInvolved)
        {
            if (behaviorTypeLookup == null)
            {
                InitializeBehaviorTypeLookup();
            }

            //RPGBehavior behavior = new RPGBehavior();
            Dictionary<string, string> metadataArgs = ExtractAllMetadataArgs(knot.Trim());

            //Should probably encapsulate this in a try-catch block. Oh well.

            string typeName = metadataArgs["Type"];
            metadataArgs.Remove("Type");

            Type type = behaviorTypeLookup[typeName];

            RPGBehavior behavior = Activator.CreateInstance(type) as RPGBehavior;

            behavior.BehaviorName = knot.Trim();
            //behavior.Metadata = BuildBehaviorMetadata(actorsInvolved, metadataArgs);

            var types = new Stack<Type>();

            //TODO: I can probably cache the different types, which may save me time in the long run.
            while (type != null)
            {
                types.Push(type);
                type = type.BaseType;
            }

            while (types.Count > 0)
            {
                type = types.Pop();
                if (buildMetadataCallbacks.TryGetValue(type, out var metadataCallback))
                {
                    metadataCallback.Invoke(behavior, metadataArgs, actorsInvolved);
                }

                if (buildSequenceCallbacks.TryGetValue(type, out var sequenceCallback))
                {
                    sequenceCallback.Invoke(behavior, knot, actorsInvolved);
                }

            }

            return behavior;
        }



        private static Dictionary<string, string> ExtractAllMetadataArgs(string knot)
        {
            var metadataLines = BehaviorRegistry.Instance.BehaviorStitchLines(knot + ".metadata");
            Dictionary<string, string> metadataArgs = new();
            foreach (var line in metadataLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var splitLine = line.Split('=');
                metadataArgs.Add(splitLine[0].Trim(), splitLine[1].Trim());
            }

            return metadataArgs;
        }

        public static CmdSequence BuildSequenceFromStitch(string knotStitch, Dictionary<string, Actor> actorKeywords)
        {
            
            List<CmdExpression> cmds = new();

            var lines = BehaviorRegistry.Instance.BehaviorStitchLines(knotStitch);

            // While effectsData can continue
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Perform all replacements in one pass using a match evaluator
                var replacedline = ReplaceActorKeywords(actorKeywords, line);

                cmds.Add(new CmdExpression() { expression = replacedline.Trim() });
            }

            return new CmdSequence() { cmdExpressions = cmds };
        }

        
        public static string ReplaceActorKeywords(Dictionary<string, Actor> actorKeywords, string line)
        {
            // Build the regex pattern for all keywords
            string pattern = @"\$\$(\w+)";
            Regex regex = new(pattern);

            line = regex.Replace(line, match =>
            {
                string key = match.Groups[1].Value; // Extract the keyword (e.g., 'keyword' from '$$keyword')
                return actorKeywords.TryGetValue(key, out var actor)
                    ? Actor.ToLookupString(actor)  // Replace with the actor string
                    : match.Value; // Leave unchanged if no match
            });
            return line;
        }
    }
}