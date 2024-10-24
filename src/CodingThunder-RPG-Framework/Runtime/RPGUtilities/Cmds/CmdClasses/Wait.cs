
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set Duration with Parameters["Dur"]. Positive value is seconds. Negative value is frames.
	/// </summary>
	public class Wait: ICmd
	{
		public string ID { get; set; }

		public Dictionary<string,string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> onFinishCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			if (!Parameters.TryGetValue("Dur", out var durString))
			{
				UnityEngine.Debug.LogError($"WaitCmd failed. Dur arg key not found. See full list of args here:\n\t{Parameters.Keys}");
				onFinishCallback.Invoke(this);
				yield break;
				
			}

			var dur = float.Parse(durString);

			//yield return new WaitForSeconds(dur);
			//onFinishCallback.Invoke(this);

			if (dur > 0)
			{
				var timePast = 0f;

				while (timePast < dur)
				{
					yield return null;
					if (Suspended)
					{
						continue;
					}
					timePast += Time.deltaTime;
				}
			} else if (dur < 0)
			{
				int framesDur = (int) dur;
				int framesPast = 0;

				while (framesPast < framesDur)
				{
					yield return null;
					if (Suspended)
					{
						continue;
					}
					framesPast++;
				}

			}

			onFinishCallback.Invoke(this);
			yield break;
		}
	}
}