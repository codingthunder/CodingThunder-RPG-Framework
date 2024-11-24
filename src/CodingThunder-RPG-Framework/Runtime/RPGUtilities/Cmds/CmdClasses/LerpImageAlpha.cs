using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Pretty simple. Finds an Image component on the Target, changes its transparency over time.
    /// Parameters["Target"] to get the GameObject.
    /// Parameters["Value"] to set what the final opacity should be (between 0 and 1, inclusive)
    /// Parameters["Dur"] to set how long it should take. Only supports seconds because I'm feeling lazy as I write this.
    /// TODO: refactor common Cmd utility functions.
    /// Assuming the LerpColor Cmd works, this Cmd is no longer necessary.
    /// </summary>
    public class LerpImageAlpha : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public GameObject Target { get; set; }
        public float? Value { get; set; }
        public float? Dur {  get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            if (Target == null)
            {
                Target = new RPGRef<GameObject>() { ReferenceId = Parameters["Target"] };
            }
            if (Value == null)
            {
                Value = new RPGRef<float?>() { ReferenceId = Parameters["Value"] };
            }
            if (Dur == null)
            {
                Dur = new RPGRef<float?>() { ReferenceId = Parameters["Dur"] };
            }

            if (Target == null || Value == null || Dur == null) {
                Debug.LogError($"Bad LerpImageAlpha Cmd. Target = {Target}, Value = {Value}, Dur = {Dur}");
                completionCallback.Invoke(this);
                yield break;
            }

            var targetImage = Target.GetComponentInChildren<Image>();

            if (targetImage == null)
            {
                Debug.LogError($"Target {Target.name} doesn't have an Image in its hierarchy. Aborting LerpImageAlpha.");
                completionCallback.Invoke(this);
                yield break;
            }


            var targetColor = targetImage.color;

            if (Dur.Value == 0)
            {
                targetColor.a = Value.Value;
                targetImage.color = targetColor;
                completionCallback.Invoke(this);
                yield break;
            }

            var timeSinceStart = 0f;
            var initialAlpha = targetColor.a;


            while (timeSinceStart <= Dur)
            {
                if (Suspended)
                {
                    yield return null;
                    continue;
                }

                var lerpValue = Mathf.Lerp(initialAlpha, Value.Value, timeSinceStart / Dur.Value);
                targetColor.a = lerpValue;
                targetImage.color = targetColor;

                yield return null;
                timeSinceStart += Time.deltaTime;
            }

            yield break;
        }
    }
}