
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CodingThunder.RPGUtilities.Cmds
{


	/// <summary>
	/// Set target object with Parameters["Target"]
	/// Set target Type with Parameters["Type"]
	/// </summary>
	public class GetVar : ICmd
	{
		public string ID { get; set; }
		public Dictionary<string,string> Parameters { get; set; }

		public object ReturnValue { get; set; }

		public IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
		{
			Type type = Type.GetType(Parameters["Type"].Trim());
			object target = LookupResolver.Instance.Resolve(Parameters["Target"].Trim(), type);

			//Parameters["Result"] = target.ToString();
			ReturnValue = target;

			OnFinishCallback.Invoke(this);
			yield break;
		}

	}
}