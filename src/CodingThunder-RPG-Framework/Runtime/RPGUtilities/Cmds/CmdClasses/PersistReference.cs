
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// More pertinent for persisting data from Unity itself than from Ink.
	/// Instead of storing the actual data, you're storing where the Data can be found.
	/// To use it, you'll need to fetch this reference as a string, then use it as the Target in a GetVar.
	/// </summary>
	public class PersistReference : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		/// <summary>
		/// To set persisted Key, use Parameters["Key"]
		/// To set the reference, use Parameters["Reference"]
		/// </summary>
		/// <param name="completionCallback"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			var gameDataKey = Parameters["Key"];
			var reference = Parameters["Reference"];

			

			GameDataManager.Instance.RegisterData(gameDataKey, reference);

			completionCallback.Invoke(this);
			yield break;
		}
	}
}