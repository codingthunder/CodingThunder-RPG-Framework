
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Remove a GameObject. Disables the object, doesn't destroy it.
	/// To set target, set Parameters["Target"]
	/// Because any target despawned MUST be in the scene, the $$Scene root keyword is assumed,
	/// though you can add it manually if you wish.
	/// </summary>
	public class Despawn : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public GameObject Target { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			if (Target == null)
			{
				var tarString = Parameters["Target"];

				if (!tarString.StartsWith("$$Scene."))
				{
					tarString = "$$Scene." + tarString;
				}
				Target = new RPGRef<GameObject>() { ReferenceId = tarString };
			}

			Target.SetActive(false);
			//UnityEngine.Object.Destroy(target);
			completionCallback.Invoke(this);
			yield break;

		}
	}
}