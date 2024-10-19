
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set Scene name with Parameters["Scene"]
	/// </summary>
	public class LoadScene : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{

			SceneManager.LoadScene(Parameters["Scene"].Trim());
			//SceneManager.LoadScene(1);
			completionCallback.Invoke(this);
			yield break;
		}
	}
}