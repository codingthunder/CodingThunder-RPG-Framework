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
        public bool? Run { get; set; }

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
            if (Run == null && Parameters.ContainsKey("Run"))
            {
                Run = new RPGRef<bool>() { ReferenceId = Parameters["Run"] };
            }
            if (KeepGoing == null && Parameters.ContainsKey("KeepGoing"))
            {
                KeepGoing = new RPGRef<bool>() { ReferenceId = Parameters["KeepGoing"] };
            }
            KeepGoing ??= false;
            var run = Run ?? false;

            
            var move2D = Target.GetComponent<Movement2D>();

            if (move2D == null)
            {
                Debug.LogError($"Error: GameObject {Parameters["Target"]} (resolves to object {Target.name}) does not" +
                    $" have a Movement2D component. Either add the component, or use KMoveOverTime.");
            }

            var targetTransform = Target.transform;

            //Slightly less computationally costly to compare sqrMagnitude vs. magnitude.
            var difference = Position.Value - (Vector2) targetTransform.position;

            //This feels stupid, but I can't think of a better way to do it at the moment.
            while (difference.sqrMagnitude > distanceThreshold * distanceThreshold)
            {
                difference = Position.Value - (Vector2)targetTransform.position;
                if (difference.normalized != move2D.m_direction.normalized)
                {
                    var m_direction = (Position.Value - (Vector2)targetTransform.position).normalized;

                    if (run)
                    {
                        move2D.Run2D(m_direction);
                    }
                    else
                    {
                        move2D.Walk2D(m_direction);
                    }
                    
                }
                
                yield return new WaitForFixedUpdate();
            }

            if (!KeepGoing.Value)
            {
                move2D.m_speed = 0f;
            }

            Debug.Log("End of MoveTo Cmd.");
            completionCallback.Invoke(this);
            yield break;
        }
    }
}