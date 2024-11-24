using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Lerp a float variable from its current value to a target value over time.
    /// Always execute along Update, not FixedUpdate.
    /// Parameters["Target"] for what you're adjusting.
    /// Parameters["Value"] for target's end value.
    /// Parameters["Dur"] for how long it takes to Lerp.
    /// </summary>
    public class LerpF : SetVar, ICmd
    {
        public override IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            var targetString = Parameters["Target"];
            float value = new RPGRef<float>() { ReferenceId = Parameters["Value"] };
            float dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };

            ISetVarMetadata setVarMetadata = null;

            try
            {
                setVarMetadata = BuildVarMetadata(targetString);
            }
            catch (ArgumentException)
            {
                Debug.LogError($"Failed to find target variable's parent in {targetString}.");
                OnFinishCallback.Invoke(this);
                yield break;
            }
            catch (MemberAccessException)
            {
                Debug.LogError($"Found parent, but couldn't find field or property from {targetString}");
                OnFinishCallback.Invoke(this);
                yield break;
            }

            if (dur == 0f)
            {
                setVarMetadata.SetValue(value);
                OnFinishCallback.Invoke(this);
                yield break;
            }

            float timePast = 0f;
            float initialValue = (float) setVarMetadata.GetValue();
            while (timePast <= dur)
            {
                float lerpValue = Mathf.Lerp(initialValue, value, timePast / dur);
                setVarMetadata.SetValue(lerpValue);

                yield return null;
                if (Suspended)
                {
                    continue;
                }
                timePast += Time.deltaTime;
            }

            OnFinishCallback.Invoke(this);
            yield break;
        }
    }
}