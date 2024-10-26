using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System;
using UnityEngine.EventSystems;
public class StoryUI : MonoBehaviour
{
    public float secondsPerCharacter = 0.06f;
    public float waitTimeOnAuto = 0.5f;

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

	private void Awake()
	{
		//DontDestroyOnLoad(transform.parent);

        //TODO: This feels hackish. There has to be a better way to handle this.
        DontDestroyOnLoad(EventSystem.current);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {
        NarrationDisplayPanel.gameObject.SetActive(false);
        ChoicesPanel.gameObject.SetActive(false);
    }

    public void DisplayChoices(string prompt, List<string> choices)
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

        foreach (var btn in  ChoicesButtons)
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

    public void Narrate(string narrationText, string nameText, bool auto)
    {
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

    public void DisplayInput(string prompt)
    {
        throw new NotImplementedException();
    }

    private IEnumerator NarrateOverTime(string text, float timeBetweenCharacters, bool auto)
    {
        var charIndex = 0;
        NarrationText.text = "";

        for (charIndex = 0; charIndex < text.Length; charIndex++) {
			yield return new WaitForSecondsRealtime(timeBetweenCharacters);
			NarrationText.text += text[charIndex];
        }

        if (auto)
        {
            yield return new WaitForSecondsRealtime(waitTimeOnAuto);
            yield break;
        }

        nextBtn.gameObject.SetActive(true);
    } 
}
