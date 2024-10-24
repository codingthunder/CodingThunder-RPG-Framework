using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Set target GameObject with Parameters["Target"] ("Scene.MyCharacter")
	/// Set x-direction with Parameters["X"]
	/// Set y-direction with Parameters["Y"]
	/// Set speed with Parameters["Speed"]
	/// Set duration with Parameters["Dur"] (if zero, will go forever. If negative, will go for number of frames)
	/// </summary>
	public class Move: ICmd
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

			GameObject target = new RPGRef<GameObject>() { ReferenceId = Parameters["Target"] };
			float xDir = new RPGRef<float>() { ReferenceId = Parameters["X"] };
			float yDir = new RPGRef<float>() { ReferenceId = Parameters["Y"] };
			float speed = new RPGRef<float>() { ReferenceId = Parameters["Speed"] };
			float dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };

			Vector2 direction = new Vector2(xDir, yDir);

			Movement2D movement2D = target.GetComponent<Movement2D>();

			if (movement2D == null)
			{
				Debug.LogError($"Target {Parameters["Target"]} does not have the Movement2D component. Unable to move.");
				yield break;
			}

			var existing_speed = movement2D.m_speed;

			movement2D.m_direction = new Vector2(xDir, yDir);
			movement2D.m_speed = speed;
			
			
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
				//The while loop above basically does this, but also allows for Suspension.
				//yield return new WaitForSeconds(dur);
				movement2D.m_direction = new Vector2 (0f, 0f);
				movement2D.m_speed = existing_speed;
				completionCallback.Invoke(this);
				yield break;
			}
			if (dur < 0)
			{
				var frameDur = (int)dur * (-1);
				var framesPast = 0;

				while (framesPast < frameDur)
				{
					yield return null;
					if (Suspended) { continue; }
					framesPast++;
				}
				movement2D.m_direction = new Vector2(0f, 0f);
				movement2D.m_speed = existing_speed;
				completionCallback.Invoke(this);
				yield break;

			}

			while (true)
			{
				yield return null;
			}

		}
	}
}