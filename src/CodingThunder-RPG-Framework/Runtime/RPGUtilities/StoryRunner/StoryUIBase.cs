using System.Collections.Generic;
using CodingThunder.RPGUtilities.GameState;
using UnityEngine;

namespace CodingThunder.RPGUtilities.RPGStory
{
    public abstract class StoryUIBase : GameStateManaged
    {
        public float secondsPerCharacter = 0.06f;
        public float waitTimeOnAuto = 0.5f;
        public abstract void Hide();
        public abstract void DisplayChoices(string prompt, List<string> choices);
        public abstract void Narrate(string narrationText, string nameText, List<string> tags);
        public abstract void DisplayInput(string prompt);
    }
}