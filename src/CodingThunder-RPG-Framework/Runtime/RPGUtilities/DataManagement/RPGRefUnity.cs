using System;
using UnityEngine;
using System.Reflection;

namespace CodingThunder.RPGUtilities.DataManagement
{
	/// <summary>
	/// Unity doesn't like properties, so I made a Unity-compatible version of RPGRef.
	/// I haven't found a reason to actually use it yet (a side-effect of figuring things out as I go),
	/// but I'm keeping it in case a reason comes up.
	/// </summary>
	[System.Serializable]
	public class RPGRefUnity
	{
		public string ReferenceId;
		public string TypeName = "int";

		// This would be used for assigning the type in the editor
		public System.Type Type
		{
			get { return System.Type.GetType(TypeName); }
			set { TypeName = value.AssemblyQualifiedName; }
		}

		public RPGRef<T> Value<T>()// where T : class
		{
			var type = Type.GetType(TypeName);
			if (type == null || !typeof(T).IsAssignableFrom(type))
			{
				throw new ArgumentException($"Type T in RPGRefUnity<T> is not assignable from the type specified by RPGRefUnity.TypeName. T: {typeof(T).Name}, TypeName: {TypeName}");
			}
			return new RPGRef<T> { ReferenceId = ReferenceId };
		}

		public object Value()
		{
			if (string.IsNullOrEmpty(ReferenceId))
			{
				Debug.LogWarning("Accessing an RPGRefUnity with an empty reference ID. Returning null.");
				return null;
			}
			Type type = typeof(RPGRef<>).MakeGenericType(Type.GetType(TypeName));
			object instance = Activator.CreateInstance(type);

			var property = type.GetProperty("ReferenceId");

			property.SetValue(instance, ReferenceId);
			return instance;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(ReferenceId))
			{
				return null;
			}
			return Value().ToString();
		}
	}
}