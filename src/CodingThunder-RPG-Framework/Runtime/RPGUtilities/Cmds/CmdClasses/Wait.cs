
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set Duration with Parameters["Dur"]
	/// </summary>
	public class Wait: ICmd
	{
		public string ID { get; set; }

		public Dictionary<string,string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> onFinishCallback)
		{
			if (!Parameters.TryGetValue("Dur", out var durString))
			{
				UnityEngine.Debug.LogError($"WaitCmd failed. Dur arg key not found. See full list of args here:\n\t{Parameters.Keys}");
				onFinishCallback.Invoke(this);
				yield break;
				
			}

			var dur = float.Parse(durString);

			yield return new WaitForSeconds(dur);
			onFinishCallback.Invoke(this);
		}
	}
}