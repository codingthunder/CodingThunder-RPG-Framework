
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
	/// Use to set a variable on one object to either a referenceId or a variable of another object.
	/// Parameters: Source, Target
	/// There is literally no reason to use this in code, so I won't be attaching properties.
	/// In fact, you should never use this from code. It's so much damn slower than just assigning something
	/// normally.
	/// </summary>
	public class SetVar: ICmd
	{

		protected interface ISetVarMetadata
		{
			public void SetValueFromRefLookup(string referenceId);
			public void SetValue(object value);
		}

		protected class SetFieldMetadata : ISetVarMetadata
		{
			private object parent;
			private FieldInfo fieldInfo;
			private Type fieldType;

			public SetFieldMetadata(object parent, FieldInfo fieldInfo) {
				
				this.parent = parent;
                this.fieldInfo = fieldInfo;
				this.fieldType = fieldInfo.FieldType;
            }

			public void SetValueFromRefLookup(string referenceId)
			{
				var value = LookupResolver.Instance.Resolve(referenceId, this.fieldType);

				fieldInfo.SetValue(parent, value);
            }

			public void SetValue(object value)
			{
				fieldInfo.SetValue(parent, value);
			}
		}

        protected class SetPropertyMetadata : ISetVarMetadata
        {
            private object parent;
            private PropertyInfo propInfo;
            private Type propType;

            public SetPropertyMetadata(object parent, PropertyInfo propInfo)
            {

                this.parent = parent;
                this.propInfo = propInfo;
                this.propType = propInfo.PropertyType;
            }

            public void SetValueFromRefLookup(string referenceId)
            {
                var value = LookupResolver.Instance.Resolve(referenceId, this.propType);

                propInfo.SetValue(parent, value);
            }

            public void SetValue(object value)
            {
                propInfo.SetValue(parent, value);
            }
        }

		protected ISetVarMetadata BuildVarMetadata(string referenceId)
		{
            var targetChain = referenceId.Split('.').ToList();

            var targetParentChain = targetChain.Take(targetChain.Count - 1).ToList();

            var parent = LookupResolver.Instance.LookupCompleteLabel(string.Join('.', targetParentChain));

            if (parent == null)
            {
				throw new ArgumentException();
            }

            var parentType = parent.GetType();

            var memberName = targetChain[targetChain.Count - 1];

            var propertyInfo = parentType.GetProperty(memberName);

			if (propertyInfo != null)
			{
				return new SetPropertyMetadata(parent, propertyInfo);
			}

			var fieldInfo = parentType.GetField(memberName);

			if (propertyInfo != null)
			{
				return new SetFieldMetadata(parent, fieldInfo);
			}

			throw new MemberAccessException();
        }

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

			ISetVarMetadata setVarMetadata = null;

			try
			{
				setVarMetadata = BuildVarMetadata(targetString);
			}
			catch (ArgumentException)
			{
				Debug.LogError($"Failed to find target variable's parent in {sourceString}.");
				OnFinishCallback.Invoke(this);
				yield break;
			}
			catch (MemberAccessException)
			{
				Debug.LogError($"Found parent, but couldn't find field or property from {sourceString}");
				OnFinishCallback.Invoke(this);
			}

			setVarMetadata.SetValueFromRefLookup(sourceString);
			OnFinishCallback.Invoke(this);
			yield break;
		}
	}
}