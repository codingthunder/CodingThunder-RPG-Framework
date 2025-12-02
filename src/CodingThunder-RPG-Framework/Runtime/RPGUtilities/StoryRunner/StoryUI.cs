using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using CodingThunder.RPGUtilities.GameState;

namespace CodingThunder.RPGUtilities.RPGStory
{
    public class StoryUI : StoryUIBase
    {
        public RectTransform NarrationDisplayPanel;
        public RectTransform NarrationPanel;
        public RectTransform NamePanel;
        public TextMeshProUGUI NarrationText;
        public TextMeshProUGUI NameText;
        public Button nextBtn;

        public RectTransform ChoicesPanel;
        public RectTransform ChoiceBtnsPanel;
        public List<Button> ChoicesButtons;
        public RectTransform PromptPanel;
        public TextMeshProUGUI PromptText;
        
        public RectTransform InfoMsgPanel;
        public TextMeshProUGUI InfoMsgText;

        public static StoryUI Instance;
        
        protected override void OnAwake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            
        }

        // Start is called before the first frame update
        void Start()
        {
            // SceneDataManager.Instance.RegisterDontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Hide()
        {
            NarrationDisplayPanel.gameObject.SetActive(false);
            ChoicesPanel.gameObject.SetActive(false);
        }

        public override void DisplayChoices(string prompt, List<string> choices)
        {
            ChoicesPanel.gameObject.SetActive(true);
            if (string.IsNullOrEmpty(prompt))
            {
                PromptPanel.gameObject.SetActive(false);
            }
            else
            {
                PromptPanel.gameObject.SetActive(true);
                PromptText.text = prompt;
            }

            foreach (var btn in ChoicesButtons)
            {
                btn.gameObject.SetActive(false);
            }

            for (int i = 0; i < choices.Count; i++)
            {
                if (i >= choices.Count)
                {
                    break;
                }
                var btn = ChoicesButtons[i];
                btn.gameObject.SetActive(true);

                var btnTextObj = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTextObj != null)
                {
                    btnTextObj.text = choices[i];
                }
            }
        }

        public override void Narrate(string narrationText, string nameText, List<string> tags)
        {
            if (nameText.Trim() == "Info")
            {
                InfoMsgText.text = narrationText;
                InfoMsgPanel.gameObject.SetActive(true);
                return;
            }
            
            var auto = tags.Contains("auto");
            
            nextBtn.gameObject.SetActive(false);
            NarrationDisplayPanel.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(nameText))
            {
                NamePanel.gameObject.SetActive(false);
            }
            else
            {
                NamePanel.gameObject.SetActive(true);
                NameText.text = nameText;
            }

            NarrationPanel.gameObject.SetActive(true);

            StartCoroutine(NarrateOverTime(narrationText, secondsPerCharacter, auto));
        }

        public override void DisplayInput(string prompt)
        {
            throw new NotImplementedException();
        }

        // TODO: Account for the fact this is gamestateManaged now...
        private IEnumerator NarrateOverTime(string text, float timeBetweenCharacters, bool auto)
        {
            var charIndex = 0;
            NarrationText.text = "";

            for (charIndex = 0; charIndex < text.Length; charIndex++)
            {
                yield return new WaitForSecondsRealtime(timeBetweenCharacters);
                NarrationText.text += text[charIndex];
            }

            if (auto)
            {
                yield return new WaitForSecondsRealtime(waitTimeOnAuto);
                GameRunner.Instance.storyRunner.Next();
                yield break;
            }

            nextBtn.gameObject.SetActive(true);
        }
    }
}