
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	public class PersistData: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

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

			var gameDataKey = Parameters["Key"];
			var target = Parameters["Target"];
			var targetTypeString = Parameters["Type"];
			Type type = Type.GetType(targetTypeString);

			Debug.LogWarning($"Persisting data to key: {gameDataKey}, target: {target}, typeString: {targetTypeString}, type: {type.FullName}");

			var targetInstance = LookupResolver.Instance.Resolve(target, Type.GetType(targetTypeString));

			GameDataManager.Instance.RegisterData(gameDataKey, targetInstance);

			completionCallback.Invoke(this);
			yield break;

		}
	}
}