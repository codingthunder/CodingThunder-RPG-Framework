using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    public class MoveTo : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public GameObject Target { get; set; }
        public Vector2? Position { get; set; }
        public float? Speed { get; set; }

        public bool? KeepGoing { get; set; }

        private float distanceThreshold = 0.1f;

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
            if (Position == null)
            {
                Position = new RPGRef<Vector2>() { ReferenceId = Parameters["Position"] };
            }
            if (Speed == null)
            {
                Speed = new RPGRef<float>() { ReferenceId = Parameters["Speed"] };
            }
            if (KeepGoing == null)
            {
                KeepGoing = new RPGRef<bool>() { ReferenceId = Parameters["KeepGoing"] };

                if (KeepGoing == null)
                {
                    KeepGoing = false;
                }
            }

            var move2D = Target.GetComponent<Movement2D>();

            if (move2D == null)
            {
                Debug.LogError($"Error: GameObject {Parameters["Target"]} (resolves to object {Target.name}) does not" +
                    $" have a Movement2D component. Either add the component, or use KMoveOverTime.");
            }

            var targetTransform = Target.transform;

            //Slightly less computationally costly to compare sqrMagnitude vs. magnitude.
            var difference = Position.Value - (Vector2) targetTransform.position;
            move2D.m_speed = Speed.Value;

            //This feels stupid, but I can't think of a better way to do it at the moment.
            while (difference.sqrMagnitude > distanceThreshold * distanceThreshold)
            {
                difference = Position.Value - (Vector2)targetTransform.position;
                if (difference.normalized != move2D.m_direction.normalized)
                {
                    move2D.m_direction = Position.Value - (Vector2)targetTransform.position;
                }
                yield return new WaitForFixedUpdate();
            }

            if (!KeepGoing.Value)
            {
                move2D.m_speed = 0f;
            }

            completionCallback.Invoke(this);
            yield break;
        }
    }
}