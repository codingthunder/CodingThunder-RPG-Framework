
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{

	/// <summary>
	/// Stores data as-is on the GameData dictionary.
	/// Primitives are copied, objects are stored by reference.
	/// That means changes you make to the data object will be persisted in the GameData dictionary as well.
	/// Useful for things like storing a character's current stats.
	/// </summary>
	public class PersistData: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		/// <summary>
		/// If Target is set via code, Type is unnecessary. Ergo,
		/// you only need Type for when it's written out.
		/// TODO: Revisit how this is structured.
		/// </summary>
		public object Target { get; set; }

		/// <summary>
		/// How is this going to be found in the persisted data?
		/// Note: Keys cannot have labels in them. They are stored as-is.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// To set persisted Key, use Parameters["Key"]
		/// To select the data to be persisted, use Parameters["Target"]
		/// To set the type for the persisted data, use Parameters["Type"]
		/// </summary>
		/// <param name="completionCallback"></param>
		/// <returns></returns>
		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			if (Key == null)
			{
				Key = Parameters["Key"];
			}

			if (Target != null)
			{
				GameDataManager.Instance.RegisterData(Key, Target);
				completionCallback.Invoke(this);
				yield break;
			}

			var type = Type.GetType(Parameters["Type"]);

			Target = LookupResolver.Instance.Resolve(Parameters["Target"], type);

			GameDataManager.Instance.RegisterData(Key, Target);

			completionCallback.Invoke(this);
			yield break;

		}
	}
}