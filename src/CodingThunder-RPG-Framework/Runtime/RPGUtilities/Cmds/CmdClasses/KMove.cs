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
	/// Set target GameObject with Parameters["Target"] ("SceneName.MyCharacter")
	/// Set direction in degrees with Parameters["Dir"] (0 degrees = UP)
	/// Set speed with Parameters["Speed"]
	/// Set distance with Parameters["Dist"] (measured in Unity Units)
	/// </summary>
	public class KMove: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public GameObject Target { get; set; }
		/// <summary>
		/// Angles! Defaults 0 to UP!
		/// </summary>
		public float? Dir {  get; set; }
		/// <summary>
		/// Unity Units per second
		/// </summary>
		public float? Speed { get; set; }
		/// <summary>
		/// Measured in Unity Units.
		/// </summary>
		public float? Dist { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			if (Target == null)
			{
				Target = new RPGRef<GameObject> { ReferenceId = Parameters["Target"] };
			}
			if (Dir == null)
			{
				Dir = new RPGRef<float> { ReferenceId = Parameters["Dir"] };
			}
			if (Speed == null)
			{
				Speed = new RPGRef<float> { ReferenceId = Parameters["Speed"] };
			}
			if (Dist == null)
			{
				Dist = new RPGRef<float> { ReferenceId = Parameters["Dist"] };
			}

			Transform targetTransform = Target.transform;

			Vector2 direction = AngleToVector2(Dir.Value);

			Vector2 moveVector = direction.normalized * Speed.Value;
			Rigidbody2D rb = Target.GetComponent<Rigidbody2D>();

			var isRbKinematic = true;

			if (rb != null)
			{
				isRbKinematic = rb.isKinematic;
				rb.isKinematic = true;
			}

			var distTraveled = 0f;


			while (distTraveled < Dist)
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

				//Based on what I'm seeing, this SHOULD work even if the object is a subobject of a moving object.
				//We shall see...
				Vector2 currentPos = targetTransform.position;

				Vector2 newPos = currentPos + (moveVector * timeSinceLastUpdate);

				targetTransform.position = newPos;

				distTraveled += (newPos - currentPos).magnitude;
			}

			//Depending on framerate, the object may overshoot target. May want to implement correction.

			if (rb != null)
			{
				rb.isKinematic = isRbKinematic;
			}

			completionCallback.Invoke(this);
			yield break;

		}

		/// <summary>
		/// Converts an angle to a Vector2. Remember: 0 degrees = UP.
		/// </summary>
		/// <param name="angleDegrees">DEFAULT 0 degrees = UP</param>
		/// <returns></returns>
		public static Vector2 AngleToVector2(float angleDegrees)
		{
            float angleRadians = Mathf.Deg2Rad * angleDegrees;

            // Calculate the x and y components
            float x = Mathf.Sin(angleRadians);
            float y = Mathf.Cos(angleRadians);

            // Return the resulting Vector2 (x, y)
            return new Vector2(x, y);
        }
	}
}
