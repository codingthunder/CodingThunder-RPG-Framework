
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
	/// Set prefab path with Parameters["Target"] (exclude "Assets" or "Resources" in your targetId)
	/// Set if the object is immediately enabled with Parameters["Enabled"]
	/// Set the object's position with Parameters["Pos"]
	/// Access the newly instantiated Parameters["Result"]
	/// </summary>
	public class LoadPrefab : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			string prefabId = new RPGRef<string>() { ReferenceId = Parameters["Target"] };
			Vector2 position = new Vector2();
			bool enabled = false;

			if (Parameters.TryGetValue("Pos", out var posString)){
				position = new RPGRef<Vector2>() { ReferenceId = posString };
			}

			//if (Parameters.TryGetValue("X", out var xString))
			//{
			//	x = new RPGRef<float>() { ReferenceId = xString };
			//}
			//if (Parameters.TryGetValue("Y", out var yString))
			//{
			//	y = new RPGRef<float>() { ReferenceId = yString };
			//}
			if (Parameters.TryGetValue("Enabled", out var enabledString))
			{
				enabled = new RPGRef<bool>() { ReferenceId = enabledString };
			}

			var instance = UnityEngine.Object.Instantiate(Resources.Load(prefabId), position, Quaternion.Euler(0, 0, 0)) as GameObject;
			instance.SetActive(enabled);

			ReturnValue = instance;
			completionCallback.Invoke(this);
			yield break;

		}
	}

}