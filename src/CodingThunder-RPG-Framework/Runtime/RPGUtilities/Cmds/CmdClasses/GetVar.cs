
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{


	/// <summary>
	/// Set target object with Parameters["Target"]
	/// Set target Type with Parameters["Type"]
	/// </summary>
	public class GetVar : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			Type type = ResolveType(Parameters["Type"].Trim());
			if (type == null)
			{
				Debug.LogError($"Failed to parse type in GetVar Cmd from {Parameters["Type"]}");
				OnFinishCallback.Invoke(this);
				yield break;
			}

			object target = LookupResolver.Instance.Resolve(Parameters["Target"].Trim(), type);

			//Parameters["Result"] = target.ToString();
			ReturnValue = target;

			OnFinishCallback.Invoke(this);
			yield break;
		}



		private Type ResolveType(string typeName)
		{
			// Try to resolve using the default method first
			Type type = Type.GetType(typeName);
			if (type != null)
				return type;

			// If not found, search through all loaded assemblies
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = assembly.GetType(typeName);
				if (type != null)
					return type;
			}

			return null; // Type not found
		}
	}
}