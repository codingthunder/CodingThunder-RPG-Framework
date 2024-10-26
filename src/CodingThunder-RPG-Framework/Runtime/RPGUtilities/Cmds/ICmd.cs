
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// Used to define Cmds. As long as your custom Cmd class implements this Interface, it can be used in Ink,
	/// Unity, etc. ALSO, for now, make sure your Cmd is in the namespace CodingThunder.RPGUtilities.Cmds. Hoping to
	/// fix that sooner than later.
	/// </summary>
	public interface ICmd
	{
		public string ID { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public object ReturnValue { get; set; }

		public bool Suspended { get; set; }
		
		IEnumerator ExecuteCmd(Action<ICmd> completionCallback);
	}
}