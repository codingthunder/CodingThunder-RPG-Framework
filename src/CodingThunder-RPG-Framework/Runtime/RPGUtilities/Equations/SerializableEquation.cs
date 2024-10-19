using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities
{
	[Serializable]
	public class SerializableEquation
	{

		public string equationText;
		private Equation _equation;
		private string _lastParsedEquationText;

		
		public float? MostRecentValue { 
			get 
			{
				RefreshIfNecessary();
				return _equation.MostRecentValue;
			} 
		}

		public List<string> Equations { get
			{
				RefreshIfNecessary ();
				return _equation.EquationsCopy;
			} }

		public List<string> Labels { get
			{
				RefreshIfNecessary ();
				return _equation.LabelsCopy; 
			} }

		public float Solve(Dictionary<string, float> labelValues)
		{
			RefreshIfNecessary();
			return _equation.Solve(labelValues);
		}

		private void RefreshIfNecessary(bool validate = true)
		{
			if (equationText == null || equationText != _lastParsedEquationText)
			{
				//May need to put this in a try-catch. For now, let's just see how it goes.
				_equation = new Equation(equationText, validate);
				_lastParsedEquationText = equationText;
			}
		}

		public static SerializableEquation Parse(string equationText)
		{
			return new SerializableEquation() { equationText = equationText };

		}

		public override string ToString()
		{
			return base.ToString();
		}

	}
}