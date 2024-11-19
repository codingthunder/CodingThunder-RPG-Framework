using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.DataManagement
{

	public static class Vector2Resolver
	{
		public static object ResolveVector2(string reference, Dictionary<string, object> labelValues)
		{
			if (!reference.Contains(','))
			{
				return DynamicExpressoEvaluator.Instance.EvaluateExpression(reference, labelValues, typeof(Vector2));
			}

			var xy = reference.Split(',');

			//This expects you to be writing in the following format: (x,y)
			var strippedXString = xy[0].Substring(1).Trim();
			var strippedYString = xy[1].Substring(1, xy[1].Length - 2).Trim();

			Debug.Log($"Resolving vector2 from string with x {strippedXString} and y {strippedYString}");

			float xPos = new RPGRef<float>() { ReferenceId = strippedXString };
			float yPos = new RPGRef<float>() { ReferenceId = strippedYString };
			return new Vector2(xPos, yPos);
		}

		public static string Vector2ToString(Vector2 vector)
		{
			return vector.ToString("F4");
		}
	}

}