
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// These are human-written Cmds. At runtime, they will be parsed into actual Cmds.
	/// If at any point you are writing a Cmd out, whether in Ink or Unity's editor,
	/// you're actually writing a CmdExpression.
	/// CmdExpressions are usually only parsed right before the Cmd is Executed. This allows
	/// classes that are running Cmds (such as Sequences) to pass data between Cmds.
	/// </summary>
	[Serializable]
	#if UNITY_EDITOR
		[UnityEditor.CustomPropertyDrawer(typeof(CmdExpression))]
	#endif
	public class CmdExpression
	{
		private static Dictionary<string, Type> cmdTypeLookup = null;

		public string expression;
		public CmdExpression() {
			if (!Application.isPlaying)
			{
				return;
			}
			if (cmdTypeLookup == null)
			{
				InitializeCmdTypeLookup();
			}
		}

		private void InitializeCmdTypeLookup()
		{
			cmdTypeLookup = new Dictionary<string, Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();

				var cmdTypes = types.Where(type => typeof(ICmd).IsAssignableFrom(type) && !type.IsAbstract);

				foreach (Type type in cmdTypes)
				{
					if (!cmdTypeLookup.ContainsKey(type.Name))
					{
						cmdTypeLookup.Add(type.Name, type);
					}
					else
					{
						Debug.LogWarning($"Duplicate command type name found: {type.Name}. Skipping.");
					}
				}

			}

			foreach (var key in  cmdTypeLookup.Keys)
			{
				Debug.Log($"Registered command: {key}");
			}
		}

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
				return null;
			}
			//Type type = Type.GetType("CodingThunder.RPGUtilities.Cmds." + args["Cmd"]);
			Type type = null;
			try
			{
				type = cmdTypeLookup[args["Cmd"]];
			}
			catch
			{
					Debug.LogError($"Bad Cmd name in CmdExpression: {expression}");
					return null;	
			}

			ICmd cmd = Activator.CreateInstance(type) as ICmd;
			cmd.Parameters = args;
			return cmd;
		}
	}

}