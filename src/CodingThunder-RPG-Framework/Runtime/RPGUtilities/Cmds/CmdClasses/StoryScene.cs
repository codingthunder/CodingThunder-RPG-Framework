
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// After using an InkTrigger (which is best), this is the second-best way to change Scenes in the Framework.
	/// You usually want to actually trigger the Unity Scene change from within Ink. Doing so consolidates all
	/// Scene initialization logic into a single location. See the ExampleScene's ink script.
	/// Set scene with Parameters["SceneName"]
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
			string storySceneId = new RPGRef<string>() { ReferenceId = Parameters["SceneName"] };

			GameRunner.Instance.StartCutscene(storySceneId);
			completionCallback.Invoke(this);
			yield break;
		}
	}
}