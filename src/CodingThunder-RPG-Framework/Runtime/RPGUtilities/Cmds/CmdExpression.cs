
using System;
using System.Linq;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
	[Serializable]
    [UnityEditor.CustomPropertyDrawer(typeof(CmdExpression))]
    public class CmdExpression
	{
		public string expression;

		/// <summary>
		/// Keys can't have labels in them, values can.
		/// Cmd args are separated by colons. I'd use commas, but those are used in so many data structures,
		/// I'm not going to write a full parser because screw that.
		/// </summary>
		/// <returns></returns>
		public ICmd ToCmd()
		{
			var args = expression.Split(':').ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

			if (!args.ContainsKey("Cmd"))
			{
				Debug.LogError($"Missing Cmd Key in CmdExpression: {expression}");
			}
			Type type = Type.GetType("CodingThunder.RPGUtilities.Cmds." + args["Cmd"]);

			if (type == null)
			{
				Debug.LogError($"Bad Cmd name in CmdExpression: {expression}");
			}

			ICmd cmd = Activator.CreateInstance(type) as ICmd;
			cmd.Parameters = args;
			return cmd;
		}
	}

}