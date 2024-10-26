using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    public class Brake : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public GameObject Target { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            if (Target == null)
            {
                Target = new RPGRef<GameObject>() { ReferenceId = Parameters["Target"] };
            }

            var movement2D = Target.GetComponent<Movement2D>();

            movement2D.m_speed = 0f;
            completionCallback.Invoke(this);
            yield break;
        }
    }
}