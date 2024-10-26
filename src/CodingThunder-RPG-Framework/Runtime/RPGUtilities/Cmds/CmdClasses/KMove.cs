using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Kinematically move something. Useful if physics doesn't affect it.
	/// Does NOT need a Rigidbody attached.
	/// Set target GameObject with Parameters["Position"] ("SceneName.MyCharacter")
	/// Set x-direction with Parameters["X"]
	/// Set y-direction with Parameters["Y"]
	/// Set speed with Parameters["Speed"]
	/// Set distance with Parameters["Dist"] (measured in Unity Units)
	/// </summary>
	public class KMove: ICmd
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

			GameObject target = new RPGRef<GameObject> { ReferenceId = Parameters["Target"] };
			float x = new RPGRef<float> { ReferenceId = Parameters["X"] };
			float y = new RPGRef<float> { ReferenceId = Parameters["Y"] };
			float speed = new RPGRef<float> { ReferenceId = Parameters["Speed"] };
			float dist = new RPGRef<float> { ReferenceId = Parameters["Dist"] };

			Transform targetTransform = target.transform;
			Vector2 moveVector = new Vector2(x, y).normalized * speed;
			Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

			var isRbKinematic = true;

			if (rb != null)
			{
				isRbKinematic = rb.isKinematic;
				rb.isKinematic = true;
			}

			var distTraveled = 0f;


			while (distTraveled < dist)
			{
				while (Suspended)
				{
					yield return null;
				}

				var timeSinceLastUpdate = 0f;
				if (rb != null)
				{
					yield return new WaitForFixedUpdate();
					timeSinceLastUpdate = Time.fixedDeltaTime;
				}
				else
				{
					yield return null;
					timeSinceLastUpdate = Time.deltaTime;
				}
				Vector2 currentPos = targetTransform.position;

				Vector2 newPos = currentPos + (moveVector * timeSinceLastUpdate);

				targetTransform.position = newPos;

				distTraveled += (newPos - currentPos).magnitude;
			}

			if (rb != null)
			{
				rb.isKinematic = isRbKinematic;
			}

			completionCallback.Invoke(this);
			yield break;

		}
	}
}
