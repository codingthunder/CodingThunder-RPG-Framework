using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder
{
    public class PauseMenu : GameStateManaged
    {
        protected override void HandleGameStateChange(GameStateEnum state)
        {
            base.HandleGameStateChange(state);

            gameObject.SetActive(IsActive);
        }
    }
}
