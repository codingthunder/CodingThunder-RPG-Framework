using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Lerp a color's alpha from its current value to a target value over time. Doing this because most colors are readonly in Unity.
    /// Always execute along Update, not FixedUpdate.
    /// Parameters["Target"] for what you're adjusting. You need to make sure this points to the component's Color field.
    /// Parameters["R"] for red, between 0 and 1.
    /// Parameters["G"] for green, between 0 and 1.
    /// Parameters["B"] for blue, between 0 and 1.
    /// Parameters["A"] for alpha, between 0 and 1.
    /// Parameters["Dur"] for how long it takes to Lerp.
    /// </summary>
    public class LerpColor : SetVar, ICmd
    {
        public override IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            var targetString = Parameters["Target"];
            float dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };

            float? r = null;
            if (Parameters.ContainsKey("R"))
            {
                r = new RPGRef<float>() { ReferenceId = Parameters["R"] };
            }

            float? g = null;
            if (Parameters.ContainsKey("G"))
            {
                g = new RPGRef<float>() { ReferenceId = Parameters["G"] };
            }

            float? b = null;
            if (Parameters.ContainsKey("B"))
            {
                b = new RPGRef<float>() { ReferenceId = Parameters["B"] };
            }

            float? a = null;
            if (Parameters.ContainsKey("A"))
            {
                a = new RPGRef<float>() { ReferenceId = Parameters["A"] };
            }


            Color finalColor = new Color();

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


            Color initialColor = (Color)setVarMetadata.GetValue();

            finalColor.r = r.HasValue ? r.Value : initialColor.r;
            finalColor.g = g.HasValue ? g.Value : initialColor.g;
            finalColor.b = b.HasValue ? b.Value : initialColor.b;
            finalColor.a = a.HasValue ? a.Value : initialColor.a;

            if (dur == 0f)
            {
                initialColor = finalColor;

                setVarMetadata.SetValue(initialColor);
                OnFinishCallback.Invoke(this);
                yield break;
            }

            float timePast = 0f;
            float initialValue = initialColor.a;
            while (timePast <= dur)
            {
                Color colorLerp = Color.Lerp(initialColor, finalColor, timePast / dur);
                //float lerpValue = Mathf.Lerp(initialValue, value, timePast / dur);
                //initialColor.a = lerpValue;
                setVarMetadata.SetValue(colorLerp);

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