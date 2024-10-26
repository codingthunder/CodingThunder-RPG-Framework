using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// uses the SimpleCameraFollow component on the main camera to have it follow the transform of a target.
    /// The Cmd itself is only active for a single frame. Will consider a longer-term Cmd for more complex Camera movments.
    /// Set Parameters["Target"] to the name of the gameObject (or a referenceId of it).
    /// </summary>
    public class CamFollow: ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            var targetString = Parameters["Target"];

            var resolvedTargetString = new RPGRef<string> { ReferenceId = targetString };

            SimpleCameraFollow cam = Camera.main.GetComponent<SimpleCameraFollow>();

            //Depending on stupid I was when coding RPGRefs, this miiiight break because it's a string. Fingers crossed, it works.
            cam.targetName = resolvedTargetString;
            completionCallback(this);
            yield break;
        }
    }
}