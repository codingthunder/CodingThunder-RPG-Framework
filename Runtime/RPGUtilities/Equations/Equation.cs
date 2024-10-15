using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data;
using Unity.VisualScripting;

namespace CodingThunder.RPGUtilities
{
	/// <summary>
	/// You use Equations to quickly create templates for calculating values, then populate those templates
	/// at runtime and call Solve to calculate the value.
	/// See documentation for examples.
	/// TODO: Make documentation.
	/// </summary>
	public class Equation
	{
		/// <summary>
		/// If Regex.IsMatch(yourEquation, equationRegex) returns true, you have a bad char in your equation.
		/// </summary>
		public static readonly string equationRegex = @"[^a-zA-Z,_0-9\.\s\(\)\$\+\-\*/]";
		/// <summary>
		/// After substituting the labels in the equation with values, we check it against this regex.
		/// Again, if Regex.IsMatch(yourEquation, subbedEquationRegex) returns true, you have a bad char in your equation.
		/// </summary>
		public static readonly string subbedEquationRegex = @"[^0-9\.\s\(\)\$\+\-\*/]";

		//Using _dataTable.Compute down below, because screw writing that manually. Making a single static instance so that I'm
		//not constantly creating and deleting a DataTable object.
		private static readonly DataTable _dataTable = new DataTable();

		private List<string> _equations;
		private List<string> _labels;
		public string FullEquation { get; private set; }
		public float? MostRecentValue { get; private set; }

		/// <summary>
		/// Use this to see how each individual equation is being split up. If you have an extra comma, this is how you'll catch it.
		/// </summary>
		public List<string> EquationsCopy { get { return _equations.ToList(); } }
		/// <summary>
		/// Use this to figure out what Labels are in your equations. Good for comparing against what you pass in.
		/// Double-check spelling and capitalization. Typos are satan.
		/// </summary>
		public List<string> LabelsCopy {  get { return _labels.ToList(); } }

		public static Equation Parse(string equation)
		{
			return new Equation(equation, false);
		}

		public override string ToString()
		{
			return FullEquation;
		}
		
		public Equation(string equationText, bool validate = true)
		{
			if (string.IsNullOrWhiteSpace(equationText))
			{
				throw new ArgumentException("Your equation is either null or empty, please try again.");
			}

			bool containsInvalidCharacters = Regex.IsMatch(equationText, equationRegex);
			if (containsInvalidCharacters)
			{
				throw new ArgumentException($"Bad equations. Check for invalid characters in: '{equationText}'");
			}

			//Because Equations are immutable, we can do all of this in the constructor.
			_labels = GenerateLabels(equationText);
			_equations = equationText.Split(',').Select(x => x.Trim()).ToList();
			//Retaining the trimmed-down version of the full equation.
			FullEquation = string.Join(", ",_equations);

			//Validate during constructor, so user knows early if they have a bad equation.
			if (validate)
			{
				ValidateEquations(this, _labels);
				this.MostRecentValue = null;
			}
		}

		/// <summary>
		/// Note: if you want to override a particular sub-equation's result,
		/// use its index as a key in your valueLabels, and your desired result as that key's value.
		/// </summary>
		/// <param name="labelValues"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public float Solve(Dictionary<string, float> labelValues)
		{
			bool allLabelsPresent = !_labels.Except(labelValues.Keys).Any();

			if (!allLabelsPresent)
			{
				throw new ArgumentException("Cannot resolve for Equation because missing labels\n\t" +
					$"Equation: {this.FullEquation}\n\t" +
					$"labelValues Keys: {labelValues.Keys}");
			}

			//TODO: Validate their labels? Nah, no need. When the equation is created, it automatically ignores stupid labels.

			for (int i = 0; i < _equations.Count; i++)
			{
				var equation = _equations[i];
				foreach (var labelValue in labelValues)
				{
					equation = equation.Replace("$" + labelValue.Key, labelValue.Value.ToString());
				}

				//If the equation at this point is empty, we would throw an error
				//But in our constructor, we should have handled any empty sub-equations already.
				//Therefore, just make sure they didn't slip in an extra standalone 'a' or something.
				bool containsInvalidCharacters = Regex.IsMatch(equation, subbedEquationRegex);
				if (containsInvalidCharacters)
				{
					throw new ArgumentException("Hey, we subbed out your Equation's labels with their values," +
						"and there's still a char in here that shouldn't be." +
						$"\n\tOriginal equation string: {_equations[i]}" +
						$"\n\tSubbed equation string: {equation}" +
						$"\n\tsubbedEquationRegex: {subbedEquationRegex}");
				}

				var solvedValue = EvaluateSubbedEquation(equation);

				//Always replaces.
				labelValues["_"] = solvedValue;

				//Won't replace if overriden.
				labelValues.TryAdd(i.ToString(), solvedValue);
			}
			//This is a bit of a hack and has the possibility of being borked later, but for now, it should work as expected.
			this.MostRecentValue = labelValues["_"];
			return (float) this.MostRecentValue;
		}

		#region PRIVATE_METHODS

		private static List<string> GenerateLabels(string fullEquation)
		{
			List<string> labels = new List<string>();
			int length = fullEquation.Length;

			for (int i = 0; i < length; i++)
			{
				//Possible TODO: Notify the dev if there's a $ without a label.
				if (fullEquation[i] == '$' && i + 1 < length && char.IsLetter(fullEquation[i + 1]))
				{
					int start = i + 1;
					int end = start;

					while (end < length && char.IsLetterOrDigit(fullEquation[end]))
					{
						end++;
					}

					string label = fullEquation.Substring(start, end - start);
					labels.Add(label);
					i = end - 1; // Movement2D the index to the end of the current label
				}
			}

			return labels;
		}

		private static void ValidateEquations(Equation equation, List<string> labels)
		{
			Dictionary<string,float> labelValues = labels.ToDictionary(x => x, x => 1f);
			try
			{
				equation.Solve(labelValues);
			}
			//Possible TODO: catch individual exceptions... But honestly, I don't want to.
			catch (Exception ex)
			{
				string errorMessage = "Hey, your equation failed to validate, so something's wrong with it.";
				throw new Exception(errorMessage, ex);
			}
		}

		private static float EvaluateSubbedEquation(string subbedEquation)
		{
			var result = _dataTable.Compute(subbedEquation, string.Empty);
			return Convert.ToSingle(result);
		}

		#endregion

	}
}