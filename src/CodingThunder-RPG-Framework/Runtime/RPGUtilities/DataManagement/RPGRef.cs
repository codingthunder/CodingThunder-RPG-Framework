
using System;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.DataManagement
{
	//The reference's type shouldn't need to be serialized. That will be tracked and stored via its declaration.
	/// <summary>
	/// In most cases, an RPGRef<T> will be treated like it is a T. Upon assignment to a non-RPGRef,
	/// it will resolve its ReferenceId, which is an expression, and will return an object which can be cast to type T.
	/// Will likely change the name ReferenceId to Expression, since Expression is more accurate.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RPGRef<T>
	{
		public string ReferenceId { get; set; }
		public T Value { get { return LookupResolver.Instance.Resolve<T>(ReferenceId); } }

		// Implicit conversion from RPGRef<T> to T
		public static implicit operator T(RPGRef<T> reference)
		{
			return reference.Value;
		}

		// Implicit conversion from T to RPGRef<T>
		public static implicit operator RPGRef<T>(T value)
		{
			return new RPGRef<T> { ReferenceId = value?.ToString() };
		}

		//public static implicit operator RPGRef<T>(RPGRefUnity reference)
		//{
		//	var referenceType = Type.GetType(reference.TypeName);
		//	if (typeof(T).IsAssignableFrom(referenceType))
		//	{
		//		return new RPGRef<T>() { ReferenceId = reference.ReferenceId };
		//	}
		//	throw new ArgumentException($"RPGRef<{typeof(T).PrefabId}> failed to be assigned from RPGRefUnity with TypeName {reference.TypeName}");
		//}

		public override bool Equals(object obj)
		{
			if (obj is RPGRef<T> otherRef)
			{
				return EqualityComparer<T>.Default.Equals(this.Value, otherRef.Value);
			}
			if (obj is T value)
			{
				return EqualityComparer<T>.Default.Equals(this.Value, value);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Value);
		}

		public static bool operator ==(RPGRef<T> left, RPGRef<T> right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(right, null) || ReferenceEquals(right, null)) return false;
			return left.Equals(right);
		}

		public static bool operator !=(RPGRef<T> left, RPGRef<T> right)
		{
			return !(left == right);
		}

		// Comparison operators (>, <, >=, <=) with runtime check for IComparable
		public static bool operator >(RPGRef<T> left, RPGRef<T> right)
		{
			if (left.Value is IComparable<T> comparable)
			{
				return comparable.CompareTo(right.Value) > 0;
			}
			throw new InvalidOperationException($"Type {typeof(T)} does not implement IComparable<T> and cannot be compared using '>' operator.");
		}

		public static bool operator <(RPGRef<T> left, RPGRef<T> right)
		{
			if (left.Value is IComparable<T> comparable)
			{
				return comparable.CompareTo(right.Value) < 0;
			}
			throw new InvalidOperationException($"Type {typeof(T)} does not implement IComparable<T> and cannot be compared using '<' operator.");
		}

		public static bool operator >=(RPGRef<T> left, RPGRef<T> right)
		{
			if (left.Value is IComparable<T> comparable)
			{
				return comparable.CompareTo(right.Value) >= 0;
			}
			throw new InvalidOperationException($"Type {typeof(T)} does not implement IComparable<T> and cannot be compared using '>=' operator.");
		}

		public static bool operator <=(RPGRef<T> left, RPGRef<T> right)
		{
			if (left.Value is IComparable<T> comparable)
			{
				return comparable.CompareTo(right.Value) <= 0;
			}
			throw new InvalidOperationException($"Type {typeof(T)} does not implement IComparable<T> and cannot be compared using '<=' operator.");
		}

		/// <summary>
		/// Will resolve its value, then return that value turned into a string.
		/// Cannot parse RPGRef<T> from a string. Must either be a declared field/property
		/// in a class, OR it needs to be built from an RPGRefUnity.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ReferenceId ?? Value?.ToString();
		}
	}
}