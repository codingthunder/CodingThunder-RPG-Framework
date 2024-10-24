
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set scene with Parameters["Scene"]
	/// </summary>
	public class StoryScene: ICmd
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
			string storySceneId = new RPGRef<string>() { ReferenceId = Parameters["Scene"] };

			GameRunner.Instance.StartCutscene(storySceneId);
			completionCallback.Invoke(this);
			yield break;
		}
	}
}