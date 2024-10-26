using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace CodingThunder.RPGUtilities.Cmds
{
	/// <summary>
	/// TODO: update to match newer development paradigms.
	/// Between CmdBlocks & CmdExpressions, you should prefer CmdExpressions for now.
	/// CmdBlocks should really only be used if the contained Cmds MUST be executed at the same time, not in sequential order.
	/// CmdBlocks will be more viable after I've gone in and updated them.
	/// </summary>
	[Serializable]
	public class CmdBlock
	{
		public string ID;
		/// <summary>
		/// How many times to run this block? Less than zero means forever, 0 means once, start counting upward.
		/// </summary>
		public int repeat = 0;
		/// <summary>
		/// Stagger your cmds by how many seconds? Zero or less has no stagger.
		/// </summary>
		public float stagger = 0;
		/// <summary>
		/// Not included in string parsing. cmdExpressions need to be added after the block is parsed.
		/// </summary>
		public List<CmdExpression> cmdExpressions;
		private int stepCount;

		public void AddCmds(List<string> cmds)
		{
			cmdExpressions.AddRange(cmds.Select(x => new CmdExpression() { expression = x}));
		}

		public void ClearCmds()
		{
			cmdExpressions.Clear();
		}

		public IEnumerator ExecuteCmdBlock(MonoBehaviour cmdRunner,Action<CmdBlock> completionCallback)
		{

			while (repeat != 0)
			{
				repeat--;

				foreach(CmdExpression cmdExpression in cmdExpressions)
				{
					var cmd = cmdExpression.ToCmd();
					cmdRunner.StartCoroutine(cmd.ExecuteCmd(Step));

					if (stagger > 0f)
					{
						yield return new WaitForSeconds(stagger);
					}
				}

				yield return new WaitUntil(() => stepCount >= cmdExpressions.Count);
				stepCount = 0;

			}
			completionCallback.Invoke(this);
			yield break;
		}

		private void Step(ICmd finishedCmd)
		{
			stepCount++;
		}

		public static CmdBlock Parse(string input)
		{
			CmdBlock block = new CmdBlock();
			var multiArgs = input.Split('\n').ToList();

			var args = multiArgs[0].Split(':').ToDictionary(x => x.Split('=')[0], x => x.Split("=")[1]);

			if (args.TryGetValue("ID", out var id))
			{
				block.ID = id;
			}
			if (args.TryGetValue("stagger", out var stagger))
			{
				block.stagger = float.Parse(stagger);
			}
			if (args.TryGetValue("repeat", out string repeat))
			{
				block.repeat = int.Parse(repeat);
			}

			multiArgs.RemoveAt(0);

			foreach (var arg in multiArgs)
			{
				if (arg.StartsWith("Cmd="))
				{
					block.cmdExpressions.Add(new CmdExpression() { expression = arg });
				}
			}

			return block;
		}
	}
}