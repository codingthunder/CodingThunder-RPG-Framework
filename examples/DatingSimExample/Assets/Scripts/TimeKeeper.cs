using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.GameState;
using CodingThunder.RPGUtilities.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodingThunder.DatingSim
{

    //Not moving this to a namespace because it's technically not part of the package.
    //It's meant to be like a script that the user would make, not a part of the framework.

    public class TimeKeeper : GameStateManaged
    {
        [Header("Use as hard override if you don't want timekeeping during gameplay.")]
        public bool suspendKeeping = false;
        [Header("Smaller the TimeScale, the faster the game goes vs. real life. 1 = real-time.")]
        public float timeScale = 60f;
        //Could get more granular, but month, day, and hour should be enough.
        //Also, there are only 360 days in this game's year.
        public int month;
        public int day;
        public int hour;

        private float timeSinceTick = 0f;

        public static TimeKeeper Instance { get; private set; }

        //Technically, doing it this way, someone could cheeze the game. So I suggest only letting them save when they sleep
        //or when there's a timejump.
        private void OnLoad(object data)
        {
            var dataDict = data as Dictionary<string, object>;
            month = (int)dataDict["month"];
            day = (int)dataDict["day"];
            hour = (int)dataDict["hour"];
            timeScale = (float)dataDict["timeScale"];
            suspendKeeping = (bool)dataDict["suspendKeeping"];
            timeSinceTick = (float)dataDict["timeSinceTick"];
        }

        private object OnSave()
        {
            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "month", month },
            { "day", day },
            { "hour", hour },
            { "suspendKeeping", suspendKeeping },
            { "timeScale", timeScale },
            { "timeSinceTick", timeSinceTick }
        };

            return data;
        }

        protected override void OnAwake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }

            Instance = this;

            base.OnAwake();

            SaveLoad.RegisterSaveLoadCallbacks("TimeKeeper", OnSave, OnLoad);
            LookupResolver.Instance.RegisterRootKeyword("TimeKeeper", GetMeFromTheDamnRootLookupBecauseJCSucksAtUsingDynamicExpressoBullshit);
        }

        private object GetMeFromTheDamnRootLookupBecauseJCSucksAtUsingDynamicExpressoBullshit(List<string> idChain)
        {
            idChain.RemoveAt(0);
            return this;
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

            timeSinceTick += Time.deltaTime * timeScale;

            //60 seconds x 60 minutes = 1 hour.
            float secondsPerHour = 60 * 60;

            if (timeSinceTick > secondsPerHour)
            {
                //I also set timeSinceTick to 0 in GoToTimeScene.
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

            GoToTimeScene();
        }

        /// <summary>
        /// Warning: Will skip any dt events before this point. If no event is scheduled, gameplay shall resume as normal.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        public void JumpToDateTime(int month, int day, int hour)
        {
            //What I'm about to do is really stupid, but I think it's the best option I've got.


            this.month = month;
            this.day = day;

            //To be clear, if the hour is set to 01:00 AM, it will be set to 00:00 AM, which while it makes sense written like this,
            //This is begging for an out of bounds exception.
            this.hour = hour - 1;

            //When gameplay resumes, the TimeKeeper will realize that timeSinceTick is greater than 3600 and kick off the increment to
            //the next hour, which in turn will trigger the next cutscene (if there is one).
            timeSinceTick = 3600;

            //SkipToTime will necessarily end the cutscene.
        }


        //TODO: Examine if Ink Flows would be a good tool for this feature.
        /// <summary>
        /// Basically, we can create an Ink Scene for any time something happens. If nothing happens, control simply returns
        /// to the player. To do a "Sleep" function, create a Sleep Scene and set the TimeScale to 0.
        /// </summary>
        private void GoToTimeScene()
        {
            timeSinceTick = 0f;
            string dt_string = $"TimeKeeper.{month}_{day}_{hour}";

            try
            {
                GameRunner.Instance.StartCutscene($"TimeKeeper.{month}_{day}_{hour}");
            }
            catch (System.Exception e)
            {
                Debug.Log("Specific time not found. Continuing gameplay.");
                //If an exception is thrown
                GameRunner.Instance.UnpauseGame();
            }



        }
    }
}