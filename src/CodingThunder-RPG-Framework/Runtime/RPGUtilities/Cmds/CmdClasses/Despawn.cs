
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
	/// </summary>
	public class Despawn : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			var tarString = Parameters["Target"];

			if (!tarString.StartsWith("$$Scene."))
			{
				tarString = "$$Scene." + tarString;
			}
			GameObject target = new RPGRef<GameObject>() { ReferenceId = tarString };

			target.SetActive(false);
			//UnityEngine.Object.Destroy(target);
			completionCallback.Invoke(this);
			yield break;

		}
	}
}