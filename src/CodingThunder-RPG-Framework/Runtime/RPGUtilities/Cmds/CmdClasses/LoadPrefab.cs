
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Loading prefabs does not use the LookupResolver because, with how it's written,
    /// the LookupResolver can't tell the difference between an active GameObject and a prefab.
    /// You can still use labels inside your Prefab targetId though.
    /// Note: In this context, "Load" means "Instantiate". "Load" is just easier to spell.
    /// Set prefab path with Parameters["PrefabId"] (exclude "Assets" or "Resources" in your prefabId)
    /// Set if the object is immediately enabled with Parameters["Enabled"]
    /// Set the object's position with Parameters["Pos"]. Note: Vector2 Parser is slightly broken, so don't include References for the x or y.
    /// Access the newly instantiated Parameters["ReturnValue"]
    /// </summary>
    public class LoadPrefab : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public object ReturnValue { get; set; }

        public bool Suspended { get; set; }

        /// <summary>
        /// What is the prefab called in the resources folder?
        /// Right now, to use the LookupResolver on a string literal, you actually need quotation marks around it.
        /// I haven't decided a workaround for it. You do NOT need quotation marks if you're looking up the string
        /// from somewhere else.
        /// </summary>
        public string PrefabId { get; set; }

        public bool? Enabled { get; set; }

        public Vector2? Pos { get; set; }

        public string Name { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            if (PrefabId == null)
            {
                PrefabId = new RPGRef<string>() { ReferenceId = Parameters["PrefabId"] };
            }

            if (Pos == null)
            {
                if (Parameters.TryGetValue("Pos", out var posString))
                {
                    Pos = new RPGRef<Vector2>() { ReferenceId = posString };
                }
            }

            if (Name == null)
            {
                if (Parameters.TryGetValue("Name", out var nameString))
                {
                    Name = new RPGRef<string>() { ReferenceId = nameString };
                }
            }

            if (Enabled == null)
            {
                if (Parameters.TryGetValue("Enabled", out var enabledString))
                {
                    Enabled = new RPGRef<bool>() { ReferenceId = enabledString };
                }
            }

            var instance = UnityEngine.Object.Instantiate(Resources.Load(PrefabId), Pos.GetValueOrDefault(), Quaternion.Euler(0, 0, 0)) as GameObject;
            instance.SetActive(Enabled.GetValueOrDefault());

            if (!string.IsNullOrEmpty(Name))
            {
                instance.name = Name;
            }


            ReturnValue = instance.name;
            completionCallback.Invoke(this);
            yield break;

        }
    }

}