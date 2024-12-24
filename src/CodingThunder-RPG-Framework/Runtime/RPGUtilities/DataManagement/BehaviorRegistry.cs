using CodingThunder.RPGUtilities.Behaviors;
using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace CodingThunder.RPGUtilities.DataManagement
{
    /// <summary>
    ///     /// This component should go on the GameRunner. It is a Singleton.
    /// What a weapon, spell, etc. does is determined by its Effect.
    /// Effects are broken into three parts:
    /// OnStart,
    /// OnTick,
    /// OnEnd,
    /// 
    /// Each of these are a CmdSequence.
    /// The BehaviorBuilder will find the correct Effect, parse its metadata and CmdSequences, and return the built object.
    /// We're kind of bastardizing Ink for this, but it seems like the best option.
    /// </summary>
    public class BehaviorRegistry : MonoBehaviour
    {
        public static BehaviorRegistry Instance { get; private set; }

        [SerializeField]
        private TextAsset behaviorsDataInkAsset;
        Story _behaviorsData;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;


            _behaviorsData = new Story(behaviorsDataInkAsset.text);

            LookupResolver.Instance.RegisterCustomToString(typeof(Actor), Actor.ToLookupString);
        }

        public List<string> BehaviorStitchLines(string knotStitch)
        {
            _behaviorsData.ChoosePathString(knotStitch);

            List<string> result = new List<string>();
            while (_behaviorsData.canContinue)
            {
                result.Add(_behaviorsData.Continue().Trim());
            }

            return result;
        }
    }
}