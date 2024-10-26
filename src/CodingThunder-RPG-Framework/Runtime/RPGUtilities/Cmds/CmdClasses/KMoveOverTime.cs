using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Moves to a location over time. Speed is determined by duration.
    /// Use with Kinematic targets or targets without rigidbodies.
    /// Parameters["Target"] to set GameObject you'll be moving.
    /// Parameters["Position"] to set target position.
    /// Parameters["Dur"] to set how long it takes.
    /// </summary>
    public class KMoveOverTime : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public GameObject Target { get; set; }

        public Vector2? Position {  get; set; }

        public float? Dur { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            if (Target == null)
            {
                Target = new RPGRef<GameObject>() { ReferenceId = Parameters["Target"] };
            }

            if (Position  == null)
            {
                Position = new RPGRef<Vector2>() { ReferenceId = Parameters["Position"] };
            }

            if (Dur == null)
            {
                Dur = new RPGRef<float> { ReferenceId = Parameters["Dur"] };
            }

            var targetTransform = Target.transform;

            var originalPosition = targetTransform.position;

            var timeSinceStart = 0f;

            while (timeSinceStart < Dur.Value)
            {
                yield return null;
                timeSinceStart += Time.deltaTime;

                var newPos = Vector2.Lerp(originalPosition, Position.Value, timeSinceStart / Dur.Value);

                targetTransform.position = newPos;
            }

            completionCallback.Invoke(this);
            yield break;

        }
    }
}