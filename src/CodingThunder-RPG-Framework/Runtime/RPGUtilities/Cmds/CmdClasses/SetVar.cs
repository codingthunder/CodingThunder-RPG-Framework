
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Use to set a variable on one object to either a value or a variable of another object.
	/// Parameters: Source, Position
	/// </summary>
	public class SetVar: ICmd
	{
		public string ID { get; set; }
		public Dictionary<string,string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
		{
			while (Suspended)
			{
				yield return null;
			}

			var sourceString = Parameters["Source"];

			var targetString = Parameters["Target"];
			var targetChain = targetString.Split('.').ToList();

			var targetParentChain = targetChain.Take(targetChain.Count - 1).ToList();

			var parent = LookupResolver.Instance.LookupCompleteLabel(string.Join('.', targetParentChain));

			if (parent == null)
			{
				Debug.LogError($"Failed to set value of {sourceString} to {targetString}. Couldn't find parent of target.");
				OnFinishCallback.Invoke(this);
				yield break;
			}

			var parentType = parent.GetType();

			var memberName = targetChain[targetChain.Count - 1];

			var propertyInfo = parentType.GetProperty(memberName);

			if (propertyInfo != null)
			{
				var propertyType = propertyInfo.PropertyType;
				var sourceValue = LookupResolver.Instance.Resolve(sourceString, propertyType);
				propertyInfo.SetValue(parent, sourceValue);
				OnFinishCallback.Invoke(this);
				yield break;
			}

			var fieldInfo = parentType.GetField(memberName);
			if (fieldInfo != null)
			{
				var fieldType = fieldInfo.FieldType;
				var sourceValue = LookupResolver.Instance.Resolve(sourceString, fieldType);
				fieldInfo.SetValue(parent, sourceValue);
				OnFinishCallback.Invoke(this);
				yield break;
			}

			UnityEngine.Debug.LogError($"Object of type {parentType.Name} does not have a field or property with the name {memberName}." +
				$"SetCmd Failed, source={sourceString}, target={targetString} ");
			
			OnFinishCallback.Invoke(this);
			yield break;
		}
	}
}