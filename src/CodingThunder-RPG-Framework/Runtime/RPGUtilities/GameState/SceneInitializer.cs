using CodingThunder.RPGUtilities.Cmds;
using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Likely going to deprecate this class out as well. Most of what it does can be handled inside of Ink.
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    public GameRunner gameRunnerPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        ExecuteInitialization();
    }


    public void ExecuteInitialization()
    {
        Debug.Log("Initializing scene.");
        if (GameRunner.Instance != null)
        {
            GameObject.Instantiate(gameRunnerPrefab);
        }
    }

}
