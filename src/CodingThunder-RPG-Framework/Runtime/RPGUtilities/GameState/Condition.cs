using System;
using UnityEngine;

namespace CodingThunder.RPGUtilities.DataManagement
{
	[Serializable]
	public class Condition
	{
		[HeaderAttribute("If left blank, always resolves to true.")]
		public string conditionExpression;

		public bool Value 
		{ 
			get 
			{ 
				if (string.IsNullOrWhiteSpace(conditionExpression))
				{
					return true;
				}
				bool? result = new RPGRef<bool?>() { ReferenceId = conditionExpression };

				if (result.HasValue)
				{
					return result.Value;
				}

				return false;
			}
		}
	}
}