using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Linq;
using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Cmds;
using System;
using CodingThunder.RPGUtilities.SaveData;
using CodingThunder.RPGUtilities.GameState;
using System.Text.RegularExpressions;

namespace CodingThunder.RPGUtilities.RPGStory
{

    /// <summary>
    /// Interacts with the InKrapper, and parses the lines from Ink.
    /// Right now, it's a bit unwieldy and probably needs to be cleaned up.
    /// </summary>
    public class StoryRunner : MonoBehaviour
    {
        private bool _storyStarted = false;

        public bool autoStart;
        public TextAsset inkAsset;
        public StoryUI storyUI;
        InKrapper inkWrapper;

        string _prompt = string.Empty;

        public event Action onSceneEnd;

        private Action switchToCutsceneCallback;


        void Awake()
        {
            inkWrapper = new InKrapper(inkAsset.text, ReceiveNextLineFromInk, ReceiveChoicesFromInk, ReceiveEndSceneFromInk, SwitchToCutscene);
            //DontDestroyOnLoad(storyUI.gameObject);

            SaveLoad.RegisterSaveLoadCallbacks("Story", GenerateStorySaveData, LoadStorySaveData);
            LookupResolver.Instance.RegisterRootKeyword("Story", LookupStoryVariable);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!_storyStarted)
            {
                storyUI.Hide();
            }
            if (autoStart)
            {
                NewStory();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// This should only ever be called once, and from the GameRunner during the Awake method.
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterCutsceneTriggerCallback(Action callback)
        {
            this.switchToCutsceneCallback = callback;
        }

        private object GenerateStorySaveData()
        {
            return inkWrapper.GetStorySaveDataJson();
        }

        private void LoadStorySaveData(object data)
        {
            inkWrapper.LoadStorySaveDataFromJson((string)data);
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

        /// <summary>
        /// Takes data from ink, processes it, and sends it to the rest of the game.
        /// </summary>
        /// <param name="line">String data from ink.</param>
        /// <param name="auto">If true, will not wait for user input on dialogue,
        /// and will continue Ink after Cmds are started, not after they finish.</param>
        private void ReceiveNextLineFromInk(string line, bool auto)
        {
            if (GameRunner.Instance.debugMode)
            {
                Debug.Log($"'{line}', auto={auto}");
            }

            //Parse Cmd
            if (line.StartsWith("Cmd="))
            {
                ICmd cmd = new CmdExpression() { expression = line }.ToCmd();

                Action<ICmd> completionCallback = auto ? OnAutoCmdComplete : OnCmdComplete;

                StartCoroutine(cmd.ExecuteCmd(completionCallback));

                if (auto)
                {
                    inkWrapper.Next();
                }
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

                Action<CmdBlock> completionCallback = auto ? OnAutoCmdBlockComplete : OnCmdBlockComplete;

                StartCoroutine(block.ExecuteCmdBlock(this, completionCallback));

                if (auto)
                {
                    inkWrapper.Next();
                }

                return;

            }

            //Parse CmdSequence
            if (line.StartsWith("CmdSequence="))
            {
                var nextLine = inkWrapper.NextLine().Trim();

                while (nextLine != "ENDSEQUENCE")
                {
                    
                    if (string.IsNullOrWhiteSpace(nextLine))
                    {
                        nextLine = inkWrapper.NextLine().Trim();
                        continue;
                    }
                    if (nextLine == null || !nextLine.StartsWith("Cmd="))
                    {
                        Debug.LogError("Hey, you forgot to close your CmdSequence! Here's what you printed instead: " + nextLine);
                        return;
                    }

                    line += "\n" + nextLine;

                    nextLine = inkWrapper.NextLine().Trim();
                }

                CmdSequence block = CmdSequence.Parse(line);

                Action<CmdSequence> completionCallback = auto ? OnAutoCmdSequenceComplete : OnCmdSequenceComplete;
                Action<CmdSequence> cancelCallback = auto ? OnAutoSequenceCancelled : OnSequenceCancelled;

                StartCoroutine(block.ExecuteCmdSequence(this, completionCallback, cancelCallback));

                if (auto)
                {
                    inkWrapper.Next();
                }

                return;

            }


            //Parse text
            var parts = Regex.Split(line, @"(?<!:):(?!:)")
                 .Select(part => part.Replace("::", ":")) // Replace escaped colons (::) with actual colons
                 .ToArray();

            if (parts.Length == 0)
            {
                inkWrapper.Next();
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
            }

            _prompt = narration;

            if (speaker != null && speaker.ToLower() == "prompt")
            {
                inkWrapper.Next();
                return;
            }

            DisplayLine(narration, speaker, auto);

        }

        private void ReceiveChoicesFromInk(List<Choice> choices)
        {
            DisplayChoice(_prompt, choices.Select(x => x.text));
        }

        private void ReceiveEndSceneFromInk()
        {
            Debug.Log("And so the story ends, a sad tale of woe and misery.");
            onSceneEnd?.Invoke();
        }

        private void SwitchToCutscene()
        {
            switchToCutsceneCallback?.Invoke();
        }


        private void OnCmdComplete(ICmd cmd)
        {
            //If I have Cmd Logic, do it here. Otherwise...
            if (cmd.ReturnValue != null)
            {
                inkWrapper.SendStoryResult(cmd.ReturnValue);
            }
            inkWrapper.Next();
        }

        private void OnAutoCmdComplete(ICmd cmd)
        {
            //Shit, I don't know if I should even be returning values here.
            //I'll allow it, just don't be surprised if it really screws things up.
            if (cmd.ReturnValue != null)
            {
                Debug.LogWarning($"WARNING: an auto {cmd.GetType().Name} Cmd is returning a value to Ink. This can cause race conditions." +
                    "Make sure you know what you're doing!!!");
                inkWrapper.SendStoryResult(cmd.ReturnValue);
            }
        }

        private void OnCmdBlockComplete(CmdBlock block)
        {
            //If I have CmdBlock Logic, do it here. Otherwise...

            inkWrapper.Next();
        }

        private void OnAutoCmdBlockComplete(CmdBlock block)
        {
            //I really do need to update CmdBlocks so that I can use them. Right now, they're kind of just... there.
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

        private void OnAutoCmdSequenceComplete(CmdSequence sequence)
        {
            //If I have CmdSequence Logic, do it here. Otherwise...
            if (sequence.localArgs.TryGetValue("_", out object value))
            {
                if (value != null)
                {
                    Debug.LogWarning("WARNING: your Auto CmdSequence is returning a value. Beware Race Conditions. Please" +
                        "consider using a non-auto CmdSequence for this Sequence.");
                    inkWrapper.SendStoryResult(value);
                }
            }
        }

        private void OnSequenceCancelled(CmdSequence sequence)
        {
            //Gotta do something or another.
            inkWrapper.Next();
        }

        private void OnAutoSequenceCancelled(CmdSequence sequence)
        {
            //Uhh... Not really anything to do here. But if we ever need it, we'll have it.
        }

        //End callbacks.


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
        public void DisplayLine(string narration, string speaker, bool auto)
        {
            storyUI.Narrate(narration, speaker, auto);
        }

        //Displays choices.
        public void DisplayChoice(string prompt, IEnumerable<string> choices)
        {
            storyUI.DisplayChoices(prompt, choices.ToList());
        }

        public void GoToChapter(string chapterID)
        {
            _storyStarted = true;
            inkWrapper.JumpToChapter(chapterID);
        }

        public void SetStoryVariable(string name, object value)
        {
            inkWrapper.SetStoryVariable(name, value);
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
}