
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set SceneName name with Parameters["SceneName"]
	/// </summary>
	public class LoadScene : ICmd
	{
        #region INTERFACE_PROPS
        public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

        #endregion

		public string SceneName {  get; set; }

        private bool sceneLoaded = false;
		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			if (string.IsNullOrEmpty(SceneName))
			{
				SceneName = Parameters["SceneName"];
			}

			SceneManager.sceneLoaded += OnSceneLoaded;

			SceneManager.LoadScene(SceneName.Trim());

			yield return new WaitUntil(() => sceneLoaded);
			
			//SceneManager.LoadScene(1);
			completionCallback.Invoke(this);
			yield break;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			sceneLoaded = true;
		}
	}
}