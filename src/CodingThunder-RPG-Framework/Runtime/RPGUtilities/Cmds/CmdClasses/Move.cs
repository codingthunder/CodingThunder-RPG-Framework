using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Uses Movement2D component to move. TODO: change Dur to Dist--Dur is a stupid way to define movement.  
	/// Set target GameObject with Parameters["Position"] ("SceneName.MyCharacter")
	/// Set direction angle (degrees) with Parameters["Dir"]
	/// Set speed with Parameters["Speed"]
	/// Set distance with Parameters["Dist"]
	/// </summary>
	public class Move: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public GameObject Target { get; set; }

		/// <summary>
		/// Angle of movement, 0 degrees = UP.
		/// </summary>
		public float? Dir {  get; set; }

		public float? Speed { get; set; }

		public float? Dist {  get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}
			if (Target == null)
			{
				Target = new RPGRef<GameObject>() { ReferenceId = Parameters["Target"] };
			}
			if (Dir == null)
			{
				Dir = new RPGRef<float>() { ReferenceId = Parameters["Dir"] };
			}
			if (Dist == null)
			{
				Dist = new RPGRef<float>() { ReferenceId = Parameters["Dist"] };
			}
			if (Speed == null)
			{
				Speed = new RPGRef<float>() { ReferenceId = Parameters["Speed"] };
			}
			//float xDir = new RPGRef<float>() { ReferenceId = Parameters["X"] };
			//float yDir = new RPGRef<float>() { ReferenceId = Parameters["Y"] };
			//float speed = new RPGRef<float>() { ReferenceId = Parameters["Speed"] };
			//float dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };

			Vector2 direction = KMove.AngleToVector2(Dir.GetValueOrDefault());

			Movement2D movement2D = Target.GetComponent<Movement2D>();

			if (movement2D == null)
			{
				Debug.LogError($"Target {Target.name} does not have the Movement2D component and cannot Move." +
					$" Try using KMove" +
					$"or KMoveOverTime instead.");
				completionCallback.Invoke(this);
				yield break;
			}

			var existing_speed = movement2D.m_speed;

			movement2D.m_direction = direction;
			movement2D.m_speed = Speed.GetValueOrDefault();

			Vector2 originalPosition = movement2D.transform.position;
			float distanceMoved = 0f;

			while (distanceMoved < Dist.GetValueOrDefault())
			{
				if (Suspended)
				{
					yield return null;
					continue;
				}
				yield return new WaitForFixedUpdate();

				distanceMoved =  ((Vector2) movement2D.transform.position - originalPosition).magnitude;
			}

			movement2D.m_direction = new Vector2(0, 0);
			movement2D.m_speed = existing_speed;
			//movement2D.m_speed = 0f;
			Debug.LogWarning("Completed MoveCmd.");
			completionCallback.Invoke(this);
			yield break;
		}
	}
}