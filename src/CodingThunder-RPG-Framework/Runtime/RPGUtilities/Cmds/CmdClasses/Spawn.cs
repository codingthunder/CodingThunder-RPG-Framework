
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Will not instantiate an object. Only sets it to active.
	/// Considering deprecation. May be redundant.
	/// To set target, set Parameters["Position"]
	/// </summary>
	public class Spawn : ICmd
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

			target.SetActive(true);
			//UnityEngine.Object.Destroy(target);
			completionCallback.Invoke(this);
			yield break;

		}
	}
}