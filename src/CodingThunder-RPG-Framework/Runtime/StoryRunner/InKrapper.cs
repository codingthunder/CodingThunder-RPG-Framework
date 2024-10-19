using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Linq;
using UnityEditor.UI;
/// <summary>
/// TODO: keep this wrapper. I can always add more mechanics to it later.
/// </summary>
public class InKrapper
{
    Story _inkStory;
    Action narrate;//Why is this here again?

    public Action<List<Choice>> ChoiceCallback;
    public Action<string> ContinueCallback;
    public Action EndSceneCallback;

    private string _prompt = null;
    public InKrapper(string storyText, Action<string> continueCallback, Action<List<Choice>> choiceCallback, Action endSceneCallback)
    {
        this.ChoiceCallback = choiceCallback;
        this.ContinueCallback = continueCallback;
        this.EndSceneCallback = endSceneCallback;
        _inkStory = new Story(storyText);
    }

    public string GetStorySaveDataJson()
    {
        return _inkStory.state.ToJson();
    }

    public void LoadStorySaveDataFromJson(string data)
    {
        _inkStory.state.LoadJson(data);
    }

    public void BeginStory()
    {
        _inkStory.ChoosePathString("main");
        Next();
    }

    public void Next()
    {
        if (_inkStory.canContinue)
        {
            ContinueCallback(_inkStory.Continue());
            return;
        }

        if (_inkStory.currentChoices.Count > 0)
        {
            ChoiceCallback(_inkStory.currentChoices);
            return;
        }

        EndSceneCallback();

    }

    public void MakeChoice(int choiceIndex)
    {
        _inkStory.ChooseChoiceIndex(choiceIndex);
        Next();
    }

    public string NextLine()
    {
        if (_inkStory.canContinue)
        {
            return _inkStory.Continue();
        }
        return null;
    }

    public void JumpToChapter(string chapterName)
    {
        _inkStory.ChoosePathString(chapterName);
        Next();
    }

    public object GetStoryVariable(string variableName)
    {
        return _inkStory.variablesState[variableName];
    }

    /// <summary>
    /// Because Ink variable types are immutable, I have the following global variables in Ink:
    /// result_int
    /// result_float
    /// result_string
    /// result_bool
    /// </summary>
    /// <param name="value"></param>
    public void SendStoryResult(object value)
    {
        if (value.GetType() == typeof(string))
        {
            _inkStory.variablesState["result_string"] = (string)value;
            return;
        }
        if (value.GetType() == typeof(int))
        {
            _inkStory.variablesState["result_int"] = (int)value;
            return;
        }
        if (value.GetType() == typeof(float))
        {
            _inkStory.variablesState["result_float"] = (float)value;
            return;
        }
        if (value.GetType() == typeof(bool))
        {
            _inkStory.variablesState["result_bool"] = (bool)value;
            return;
        }
    }

  //  private void ParseContinue(string text)
  //  {
  //      var parts = text.Split(':');

  //      if (parts.Length == 0 )
  //      {
  //          return;
  //      }

  //      //Eventually, we'll use : to delimit commands.

  //      string narration = "";
  //      string speaker = null;

  //      if (parts.Length == 1) {
  //          narration = parts[0];
  //      }
  //      if (parts.Length == 2 )
  //      {
  //          speaker = parts[0];
  //          narration = parts[1];
			
  //          if (speaker.ToLower() == "prompt")
		//	{
		//		this._prompt = speaker;
		//	}
		//}

  //      _gsActions.DisplayLine(narration, speaker);
  //  }
}
