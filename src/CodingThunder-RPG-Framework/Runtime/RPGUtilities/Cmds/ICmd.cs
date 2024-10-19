
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	public interface ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }
		
		IEnumerator ExecuteCmd(Action<ICmd> completionCallback);
	}
}