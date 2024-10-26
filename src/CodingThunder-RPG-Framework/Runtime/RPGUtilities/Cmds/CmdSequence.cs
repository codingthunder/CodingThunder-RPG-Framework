
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;

namespace CodingThunder.RPGUtilities.Cmds
{
	[Serializable]
	public class CmdSequence
	{
		/// <summary>
		/// Qualifies if this Sequence should run or not. Leave blank to always return true.
		/// </summary>
		public Condition condition;

		public string ID;
		/// <summary>
		/// How many times to run this block? Less than zero means forever, 0 means once, start counting upward.
		/// </summary>
		public int repeat = 1;
		/// <summary>
		/// Delay each command by how many seconds? Zero or less has no delay.
		/// </summary>
		public float delay = 0;
		/// <summary>
		/// Not included in string parsing. cmdExpressions need to be added after the block is parsed.
		/// </summary>
		public List<CmdExpression> cmdExpressions;
		private int stepCount;

		private bool suspended = false;
		private HashSet<ICmd> activeCmds = new HashSet<ICmd>();

		public Dictionary<string, object> localArgs = new Dictionary<string, object>();

		public void AddCmds(List<string> cmds)
		{
			if (cmdExpressions == null)
			{
				cmdExpressions = new List<CmdExpression>();
			}

			cmdExpressions.AddRange(cmds.Select(x => new CmdExpression() { expression = x }));
		}

		public void ClearCmds()
		{
			cmdExpressions.Clear();
		}

		public void SetIsSuspended(bool isSuspended)
		{
			suspended = isSuspended;

			foreach(var cmd in activeCmds)
			{
				cmd.Suspended = isSuspended;
			}
		}


		public IEnumerator ExecuteCmdSequence(MonoBehaviour cmdRunner, Action<CmdSequence> completionCallback, Action<CmdSequence> cancelCallback)
		{
			if (condition != null && !condition.Value)
			{

				if (GameRunner.Instance.debugMode) {
                    Debug.Log($"CmdSequence condition was not met. Cancelling. No completionCallback. Do not pass go.");
                }
				
				//TODO: Possibly add a cancellation Callback.
				cancelCallback.Invoke(this);
				yield break;
			}

			Debug.Log("Executing CmdSequence");
			Debug.Log($"Repeat: {repeat}");
			Debug.Log($"ExpressionCount: {cmdExpressions.Count}");

			while (suspended)
			{
				yield return null;
			}

			while (repeat != 0)
			{

				stepCount = 0;
				repeat--;

				foreach (CmdExpression cmdExpression in cmdExpressions)
				{
					Debug.Log($"Executing CmdExpression {cmdExpression.expression}");
					foreach (var arg in localArgs)
					{
						cmdExpression.expression = cmdExpression.expression.Replace("$$" + arg.Key, LookupResolver.Instance.Stringify(arg.Value));
					}

					var timeDelayed = 0f;

					while (delay > 0f && timeDelayed < delay)
					{
						yield return null;

                        if (suspended)
						{
							continue;
						}

						timeDelayed += Time.deltaTime;
					}

					//if (delay > 0f)
					//{
					//	yield return new WaitForSeconds(delay);
					//}
					var stepNumber = stepCount;

					var cmd = cmdExpression.ToCmd();
					cmd.ID = stepNumber.ToString();

					activeCmds.Add(cmd);

					var cmdCoroutine = cmdRunner.StartCoroutine(cmd.ExecuteCmd(Step));

					yield return new WaitUntil(() => stepCount > stepNumber);
					yield return null;
					
				}

			}
			completionCallback.Invoke(this);
			yield break;
		}

		private void Step(ICmd finishedCmd)
		{
			//if (finishedCmd.Parameters.TryGetValue("Result", out string value)) {
			//	localArgs["_"] = value;
			//	localArgs[finishedCmd.ID] = value;
			//}

			activeCmds.Remove(finishedCmd);

			if (finishedCmd.ReturnValue != null)
			{
				localArgs["_"] = finishedCmd.ReturnValue;
				localArgs[finishedCmd.ID] = finishedCmd.ReturnValue;
			}

			stepCount++;
		}

		public static CmdSequence Parse(string input)
		{
			CmdSequence block = new CmdSequence();

			var multiArgs = input.Split('\n').ToList();

			var args = multiArgs[0].Split(':').ToDictionary(x => x.Split('=')[0], x => x.Split("=")[1]);

			if (args.TryGetValue("ID", out var id))
			{
				block.ID = id;
			}
			if (args.TryGetValue("delay", out var delay))
			{
				block.delay = float.Parse(delay);
			}
			if (args.TryGetValue("repeat", out string repeat))
			{
				block.repeat = int.Parse(repeat);
			}
			if (args.TryGetValue("Condition", out var condition))
			{
				block.condition = new Condition() { conditionExpression = condition };
			}

			multiArgs.RemoveAt(0);

			foreach( var arg in multiArgs)
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