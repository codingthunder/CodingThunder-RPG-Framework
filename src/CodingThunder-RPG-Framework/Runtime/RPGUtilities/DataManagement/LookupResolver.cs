
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CodingThunder.RPGUtilities.DataManagement
{
	/// <summary>
	/// An actual monstrosity. Unless you know what you're doing, avoid messing with for now. Focus on using RPGRefs.
	/// Only exception, maybe, is registering custom root keywords, custom resolvers, and custom toStrings.
	/// </summary>
	public class LookupResolver
	{
		private static readonly Lazy<LookupResolver> _instance = new(() => new LookupResolver());

		public readonly string labelRegex = @"\$\$([a-zA-Z0-9\._]+)";

		public static LookupResolver Instance => _instance.Value;

		//private Dictionary<Type, Func<string, object>> _resolvers;
		private Dictionary<Type, Func<string, Dictionary<string, object>, object>> _customResolvers = new();
		private Dictionary<string, Func<List<string>, object>> _rootKeywords = new();
		private Dictionary<Type, Func<object, string>> _customToStrings = new();

		public LookupResolver()
		{
			RegisterResolver(typeof(Vector2), Vector2Resolver.ResolveVector2);
		}

		public void RegisterResolver(Type type, Func<string, Dictionary<string, object>, object> resolver)
		{
			_customResolvers[type] = resolver;
		}

		public bool HasCustomResolver(Type type)
		{
			return _customResolvers.ContainsKey(type);
		}

		public void RegisterRootKeyword(string keyword, Func<List<string>,object> resolver)
		{
			_rootKeywords[keyword] = resolver;
		}

		public bool HasRootKeyword(string keyword)
		{
			return _rootKeywords.ContainsKey(keyword);
		}

		public void RegisterCustomToString(Type type, Func<object, string> _customToString)
		{
			_customToStrings[type] = _customToString;
		}

		public bool HasCustomToString(Type type)
		{
			return _customToStrings.ContainsKey(type);
		}

		public T Resolve<T>(string reference)
		{
			var value = Resolve(reference, typeof(T));

			if (value == null)
			{
				return default;
			}

			if (typeof(T).IsAssignableFrom(value.GetType()))
			{
				return (T)value;
			}

			return (T)Convert.ChangeType(value, typeof(T));
		}

		public string Stringify(object value)
		{
			if (_customToStrings.TryGetValue(value.GetType(), out var result))
			{
				return result(value);
			}

			return value.ToString();
		}

		public object Resolve(string reference, Type typeForT)
		{
			if (string.IsNullOrEmpty(reference))
			{
				return GetDefaultValue(typeForT);
			}
			
			var labelValues = ResolveLabels(reference);

			reference = reference.Replace("$$", "");

			//TODO: make this commented code work.
			//if (labelValues.Count == 1)
			//{
			//	var key = labelValues.Keys.FirstOrDefault();

			//	if (key.Trim() == reference.Trim())
			//	{
			//		return labelValues[key];
			//	}
			//}

			if (typeForT != null && _customResolvers.TryGetValue(typeForT, out var resolver))
			{
				return resolver.Invoke(reference, labelValues);
			}

			//foreach (var pair in labelValues)
			//{
			//	reference = reference.Replace("$$" + pair.Key, Stringify(pair.Value));
			//}

			return DynamicExpressoEvaluator.Instance.EvaluateExpression(reference,labelValues, typeForT);
		}

		private static object GetDefaultValue(Type typeForT)
		{
			if (typeForT == typeof(void)) return null;

			if (typeForT.IsValueType)
			{
				return Activator.CreateInstance(typeForT);
			}

			return null;
		}

		public Dictionary<string, object> ResolveLabels(string reference)
		{
			var labelValues = new Dictionary<string, object>();

			if (string.IsNullOrEmpty(reference))
			{
				return labelValues;
			}

			MatchCollection matches = Regex.Matches(reference, labelRegex);

			foreach (Match match in matches)
			{
				var labelString = match.Value.Substring(2);
				object value = LookupCompleteLabel(labelString);
				labelValues[labelString] = value;
			}

			return labelValues;
		}

		public object LookupCompleteLabel(string label)
		{
			if (string.IsNullOrEmpty(label))
			{
				return default;
			}

			if (label.StartsWith("$$"))
			{
				label = label.Substring(2);
			}

			var separatorIndex = label.IndexOf('.');
			var keyWord = separatorIndex < 0 ? label : label.Substring(0, separatorIndex);

			if (_rootKeywords.TryGetValue(keyWord, out var kw_resolver))
			{
				//We're including the keyword in case the keywordResolver wants to do something with it.
				var idChain = label.Split('.').ToList();

				int count = idChain.Count;

				var target = kw_resolver.Invoke(idChain);
				if (target == null)
				{
					//UnityEngine.Debug.LogError($"LookupCompleteLabel failed. Be sure {label} starts with a root keyword. " +
					//	$"Also, be sure to register your rootKeywordResolver.");
					return null;
				}

				if (idChain.Count == count)
				{
					idChain.RemoveAt(0);
				}

				return LookupPartialLabel(string.Join('.', idChain), target);
			}

			return DynamicExpressoEvaluator.Instance.EvaluateExpression(label, new(),typeof(object));
		}

		public object LookupPartialLabel(string lookupId, object target)
		{
			if (target == null)
			{
				Debug.LogError("Bad label target. Needs to be not null.");
				return null;
			}

			// Use Dynamic Expresso to evaluate the lookupId
			object result = DynamicExpressoEvaluator.Instance.EvaluateObjectLookup(target, lookupId);
			if (result == null)
			{
				Debug.LogError($"Failed to evaluate lookupId {lookupId} on target.");
			}

			return result;
		}
	}
}