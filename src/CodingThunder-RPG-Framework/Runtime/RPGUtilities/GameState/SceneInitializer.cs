using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Likely going to deprecate this class out as well. Most of what it does can be handled inside of Ink.
/// </summary>
public class SceneInitializer : GameStateManaged
{
    public List<CmdSequence> startupSequences = new();

    // Start is called before the first frame update
    protected override void OnStart()
    {
        ExecuteInitialization();
    }


    public void ExecuteInitialization()
    {
        Debug.Log("Initializing scene.");
        foreach (var seq in startupSequences)
        {
            StartCoroutine(seq.ExecuteCmdSequence(this, OnInitializationComplete, OnSequenceCancelled));
        }
    }

    private void OnInitializationComplete(CmdSequence cmdSequence)
    {
        //Something, I don't know.
    }

    private void OnSequenceCancelled(CmdSequence cmdSequence)
    {
        //Another something, I don't know.
    }
}
