using DynamicExpresso;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using CodingThunder.RPGUtilities.Mechanics;
using System.Text.RegularExpressions;

namespace CodingThunder.RPGUtilities.DataManagement
{
	public class DynamicExpressoEvaluator
	{
		private static DynamicExpressoEvaluator instance;
		public static DynamicExpressoEvaluator Instance { 
			get 
			{
				if (instance == null)
				{
					instance = new DynamicExpressoEvaluator();
				}
				return instance;
			}
		}

		private Interpreter _interpreter = new Interpreter();
		private HashSet<Type> _registeredTypes = new HashSet<Type>();

		// Method to register a single type if not already registered
		public void RegisterTypeIfNotRegistered(Type type)
		{
			if (!_registeredTypes.Contains(type))
			{
				_interpreter.Reference(type);
				_registeredTypes.Add(type);
			}
		}

		// Method to register all types from a given namespace
		public void RegisterNamespace(string ns)
		{

            // Get all types in the current assembly that belong to the given namespace
            //var types = Assembly.GetExecutingAssembly().GetTypes()
            //					.Where(t => t.Namespace == ns && (t.IsClass || (t.IsValueType && !t.IsPrimitive))).ToArray();

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = allAssemblies.SelectMany(assembly => assembly.GetTypes())
                                     .Where(t => t.Namespace == ns && (t.IsClass || (t.IsValueType && !t.IsPrimitive)))
                                     .ToArray();

            foreach (var type in types)
			{
				RegisterTypeIfNotRegistered(type);
			}
		}


		/// <summary>
		/// Evaluates a lookup string on a given object using Dynamic Expresso.
		/// </summary>
		/// <param name="target">The target object (e.g., a Unity component like Transform).</param>
		/// <param name="lookupString">The string to be evaluated (e.g., "transform.position.x").</param>
		/// <returns>The evaluated value from the lookup string.</returns>
		public object EvaluateObjectLookup(object target, string lookupString)
		{
			//_interpreter.SetFunction("GetComponentInChildren",
			//	new Func<GameObject, Type, Component>((go, type) => go.GetComponentInChildren(type)));
			if (lookupString.Contains(')'))
			{
				Debug.Log(lookupString);
			}

			if (target == null)
			{
				Debug.LogError("Target object is null.");
				return null;
			}
			//GameObject gameObject = target as GameObject;
			//gameObject.GetComponentInChildren(typeof(Movement2D),true);

			//GameObject gameObject = target as GameObject;
			//var pleaseWork = ((gameObject).GetComponentInChildren(typeof(Movement2D)) as Movement2D).walkingSpeed;

			if (string.IsNullOrWhiteSpace(lookupString))
			{
				return target;
			}

			if (!lookupString.StartsWith("__target."))
			{
				lookupString = "__target." + lookupString;
			}

			// Create an instance of the _interpreter
			//var _interpreter = new Interpreter();
			

			// Register the target object in the _interpreter so it can be accessed within expressions
			_interpreter.SetVariable("__target", target);

			try
			{
				// Now evaluate the lookup string, which refers to properties of the target object.
				// For example, lookupString could be "target.transform.position.x"
				return _interpreter.Eval(lookupString);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Dynamic Expresso Evaluation Error: {ex.Message}");
				return null;
			}
		}

		public object EvaluateExpression(string expression, Dictionary<string,object> labelValues, Type argumentType)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return null;
			}
			var keys = labelValues.Keys.ToArray();

			for (int i = 0; i < labelValues.Count; i++)
			{
				string key = keys[i];
                var value = labelValues[key];

				//var replacementExpression = value == null ? $"(({argumentType.FullName}) __{i})" : $"__{i}";

				//            expression = expression.Replace(key, replacementExpression);

				expression = expression.Replace(key, $"__{i}");

				//Putting this here to try to get around the fact that DynamicExpresso doesn't like comparing null with bool.
				//if (value == null)
				//{
				//	expression = Regex.Replace(expression, $@"!\s*__{i}", "!false");

				//	// Replace "null" cases for safe handling in the expression
				//	expression = Regex.Replace(expression, $@"__{i}\s*==\s*true", $"false == true");
				//	expression = Regex.Replace(expression, $@"__{i}\s*!=\s*true", $"false != true");
				//	expression = Regex.Replace(expression, $@"__{i}\s*==\s*false", $"false == false");
				//	expression = Regex.Replace(expression, $@"__{i}\s*!=\s*false", $"false == false");

				//	expression = Regex.Replace(expression, $@"true\s*==\s*__{i}", $"true == false");
				//	expression = Regex.Replace(expression, $@"true\s*!=\s*__{i}", $"true != false");
				//	expression = Regex.Replace(expression, $@"false\s*==\s*__{i}", $"false == false");
				//	expression = Regex.Replace(expression, $@"false\s*!=\s*__{i}", $"false != false");

				//}

				Type valueType = argumentType;

				if (value != null)
				{
					valueType = value.GetType();
				}

                _interpreter.SetVariable($"__{i}", value, valueType);
			}


			try
			{
				return _interpreter.Eval(expression, argumentType);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Dynamic Expresso for expression: {expression}\n\t Evaluation Error: {ex.Message}");
				return null;
			}
		}

		public T EvaluateExpression<T>(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return default;
			}

			try
			{
				return _interpreter.Eval<T>(expression);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Dynamic Expresso Evaluation Error: {ex.Message}");
				return default;
			}
		}
	}
}