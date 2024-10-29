using CodingThunder.RPGUtilities.GameState;
using CodingThunder.RPGUtilities.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not moving this to a namespace because it's technically not part of the package.
//It's meant to be like a script that the user would make, not a part of the framework.

public class TimeKeeper : GameStateManaged
{
    [Header("Use as hard override if you don't want timekeeping during gameplay.")]
    public bool suspendKeeping = false;
    [Header("How much faster is the game than real life?")]
    public float timeScale = 60f;
    //Could get more granular, but month, day, and hour should be enough.
    //Also, there are only 360 days in this game's year.
    public int month;
    public int day;
    public int hour;

    private float timeSinceTick = 0f;

    protected override void OnAwake()
    {
        base.OnAwake();

        SaveLoad.RegisterSaveLoadCallbacks("TimeKeeper", OnSave, OnLoad);
    }

    protected override void HandleDestroy()
    {
        base.HandleDestroy();
        SaveLoad.DeregisterSaveLoadCallbacks("TimeKeeper");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (suspendKeeping)
        {
            return;
        }

        timeSinceTick += Time.deltaTime;

        float secondsPerHourScaled = 60 * 60 / timeScale;

        if (timeSinceTick > secondsPerHourScaled)
        {
            timeSinceTick = 0f;
            IncrementHour();
        }
    }

    public void IncrementHour()
    {
        hour++;
        if (hour > 23)
        {
            hour = 0;
            day++;
        }
        if (day > 30)
        {
            //TODO: Check for off-by-one errors.
            day = 1;
            month++;
        }
        if (month > 12)
        {
            month = 1;
        }

        GoToTimeScene($"dt_{month}_{day}_{hour}");
    }

    /// <summary>
    /// Warning: Will skip any dt events before this point. If no event is scheduled, gameplay shall resume as normal.
    /// </summary>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    public void JumpToDateTime(int month, int day, int hour)
    {
        this.month = month;
        this.day = day;
        this.hour = hour;
        GoToTimeScene($"dt_{month}_{day}_{hour}");
    }

    //Technically, doing it this way, someone could cheeze the game. So I suggest only letting them save when they sleep
    //or when there's a timejump.
    private void OnLoad(object data)
    {
        var dataDict = data as Dictionary<string, object>;
        month = (int) dataDict["month"];
        day = (int) dataDict["day"];
        hour = (int) dataDict["hour"];
        suspendKeeping = (bool)dataDict["suspendKeeping"];
    }

    private object OnSave()
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "month", month },
            { "day", day },
            { "hour", hour },
            { "suspendKeeping", suspendKeeping }
        };

        return data;
    }

    /// <summary>
    /// Basically, we can create an Ink Scene for any time something happens. If nothing happens, control simply returns
    /// to the player.
    /// </summary>
    /// <param name="sceneName">Exclude "TimeKeeper." from scene name! I have to do some logic before adding it in.</param>
    private void GoToTimeScene(string sceneName)
    {
        GameRunner.Instance.storyRunner.SetStoryVariable("time_keeper_scene", sceneName);


        GameRunner.Instance.StartCutscene("TimeKeeper.RunTimeScene");
    }
}
