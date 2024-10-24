using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Linq;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Cmds;
using System;
using CodingThunder.RPGUtilities.SaveData;

public class StoryRunner : MonoBehaviour
{
	public bool autoStart;
	public TextAsset inkAsset;
	public StoryUI storyUI;
    InKrapper inkWrapper;

	string _prompt = string.Empty;

	public event Action onSceneEnd;


    void Awake()
	{
		inkWrapper = new InKrapper(inkAsset.text, ReceiveNextLineFromInk, ReceiveChoicesFromInk, ReceiveEndSceneFromInk);
		//DontDestroyOnLoad(storyUI.gameObject);

		SaveLoad.RegisterSaveLoadCallbacks("Story", GenerateStorySaveData, LoadStorySaveData);
		LookupResolver.Instance.RegisterRootKeyword("Story", LookupStoryVariable);
	}

	// Start is called before the first frame update
	void Start()
    {
        storyUI.Hide();
		if (autoStart)
		{
			NewStory();
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private object GenerateStorySaveData()
	{
		return inkWrapper.GetStorySaveDataJson();
	}

	private void LoadStorySaveData(object data)
	{
		inkWrapper.LoadStorySaveDataFromJson((string) data);
	}

    public object LookupStoryVariable(List<string> idChain)
	{
		if (idChain[0] == "Story")
		{
			idChain.RemoveAt(0);
		}
		var name = idChain[0];
		idChain.RemoveAt(0);
		return inkWrapper.GetStoryVariable(name);
	}

	//Gonna insert my callbacks here.
	private void ReceiveNextLineFromInk(string line)
	{

		//Parse Cmd
		if (line.StartsWith("Cmd="))
		{
			ICmd cmd = new CmdExpression() { expression = line }.ToCmd();

			StartCoroutine(cmd.ExecuteCmd(OnCmdComplete));
			return;
		}
		//Parse Cmd Block
		if (line.StartsWith("CmdBlock="))
		{
			var nextLine = "";

			while (nextLine != "ENDBLOCK")
			{
				nextLine = inkWrapper.NextLine().Trim();
				if (nextLine == null || !nextLine.StartsWith("Cmd="))
				{
					Debug.LogError("Hey, you forgot to close your CmdBlock!");
					return;
				}

				line += "\n" + nextLine;
			}

			CmdBlock block = CmdBlock.Parse(line);

			StartCoroutine(block.ExecuteCmdBlock(this, OnCmdBlockComplete));
			return;

		}

		//Parse CmdSequence
		if (line.StartsWith("CmdSequence="))
		{
			var nextLine = "";

			while (nextLine != "ENDSEQUENCE")
			{
				nextLine = inkWrapper.NextLine().Trim();
				if (nextLine == null || !nextLine.StartsWith("Cmd="))
				{
					Debug.LogError("Hey, you forgot to close your CmdSequence!");
					return;
				}

				line += "\n" + nextLine;
			}

			CmdSequence block = CmdSequence.Parse(line);

			StartCoroutine(block.ExecuteCmdSequence(this, OnCmdSequenceComplete));
			return;

		}


		//Parse text
		var parts = line.Split(':');

		if (parts.Length == 0)
		{
			return;
		}

		//We're using ':' to delimit commands as well.

		string narration = "";
		string speaker = null;

		if (parts.Length == 1)
		{
			narration = parts[0];
		}
		if (parts.Length == 2)
		{
			speaker = parts[0];
			narration = parts[1];

			if (speaker.ToLower() == "prompt")
			{
				this._prompt = narration;
				speaker = null;
			}
		}

		DisplayLine(narration, speaker);

	}

	private void ReceiveChoicesFromInk(List<Choice> choices)
	{
		DisplayChoice(_prompt, choices.Select(x => x.text));
	}

	private void ReceiveEndSceneFromInk()
	{
		Debug.Log("And so the story ends, a sad tale of woe and misery.");
		onSceneEnd.Invoke();
	}


	//End callbacks.

	private void OnCmdComplete(ICmd cmd)
	{
		//If I have Cmd Logic, do it here. Otherwise...
		if (cmd.ReturnValue != null)
		{
			inkWrapper.SendStoryResult(cmd.ReturnValue);
		}
		inkWrapper.Next();
	}

	private void OnCmdBlockComplete(CmdBlock block)
	{
		//If I have CmdBlock Logic, do it here. Otherwise...

		inkWrapper.Next();
	}

	private void OnCmdSequenceComplete(CmdSequence sequence)
	{
		//If I have CmdSequence Logic, do it here. Otherwise...
		if (sequence.localArgs.TryGetValue("_", out object value))
		{
			if (value != null)
			{
				inkWrapper.SendStoryResult(value);
			}
		}

		inkWrapper.Next();
	}


	public void NewStory()
	{
		inkWrapper.BeginStory();
	}

	//Tells the Ink to go to the next line.
	public void Next() 
	{
		//Debug.Log("Storyrunner next!");
		storyUI.Hide();
		inkWrapper.Next(); 
	}

	//Tells ink to make a decision.
	public void MakeChoice(int choiceIndex)
	{
		storyUI.Hide();
		inkWrapper.MakeChoice(choiceIndex);
	}

	//Tells the story
	public void DisplayLine(string narration, string speaker = null)
	{
		storyUI.Narrate(narration, speaker);
	}

	//Displays choices.
	public void DisplayChoice(string prompt, IEnumerable<string> choices)
	{
		storyUI.DisplayChoices(prompt, choices.ToList());
	}

	public void GoToChapter(string chapterID)
	{
		inkWrapper.JumpToChapter(chapterID);
	}

	//public void PromptInput(string prompt)
	//{
	//	throw new System.NotImplementedException();
	//}

	//public void FinishStoryControl()
	//{
	//	throw new System.NotImplementedException();
	//}

	//public void LogError(string errorText)
	//{
	//	throw new System.NotImplementedException();
	//}
}
