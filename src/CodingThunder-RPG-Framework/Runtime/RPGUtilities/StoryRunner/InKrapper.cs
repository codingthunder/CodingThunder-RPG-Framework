using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Linq;

namespace CodingThunder.RPGUtilities.RPGStory
{
    /// <summary>
    /// No, the class name isn't a typo. It's a really bad pun. Directly interacts with Ink.
    /// </summary>
    public class InKrapper
    {
        Story _inkStory;
        Action narrate;//Why is this here again?

        public Action<List<Choice>> ChoiceCallback;
        public Action<string, bool> ContinueCallback;
        public Action EndSceneCallback;

        public InKrapper(string storyText, Action<string, bool> continueCallback, Action<List<Choice>> choiceCallback, Action endSceneCallback)
        {
            ChoiceCallback = choiceCallback;
            ContinueCallback = continueCallback;
            EndSceneCallback = endSceneCallback;
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

                var lineData = _inkStory.Continue();
                var tags = _inkStory.currentTags;

                //If it's dialogue, will continue after a set amount of time.
                //If it's a Cmd or Cmd Sequence, will start the Cmd but will not wait for it to finish before Continuing.
                var auto = tags.Contains("auto");

                //Debug.Log(lineData);

                ContinueCallback(lineData, auto);
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

        public void SetStoryVariable(string variableName, object value)
        {
            _inkStory.variablesState[variableName] = value;
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
}