using CodingThunder.RPGUtilities.Cmds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            StartCoroutine(seq.ExecuteCmdSequence(this, OnInitializationComplete));
        }
    }

    public void OnInitializationComplete(CmdSequence cmdSequence)
    {
        //Something, I don't know.
    }
}
