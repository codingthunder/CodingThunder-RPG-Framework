
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	public class PersistData: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		/// <summary>
		/// To set persisted Key, use Parameters["Key"]
		/// To select the data to be persisted, use Parameters["Target"]
		/// To set the type for the persisted data, use Parameters["Type"]
		/// </summary>
		/// <param name="completionCallback"></param>
		/// <returns></returns>
		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			var gameDataKey = Parameters["Key"];
			var target = Parameters["Target"];
			var targetTypeString = Parameters["Type"];

			var targetInstance = LookupResolver.Instance.Resolve(target, Type.GetType(targetTypeString));

			GameDataManager.Instance.RegisterData(gameDataKey, target);

			completionCallback.Invoke(this);
			yield break;

		}
	}
}