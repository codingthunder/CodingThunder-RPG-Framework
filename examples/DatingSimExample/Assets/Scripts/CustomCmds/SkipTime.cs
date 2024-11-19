using CodingThunder.DatingSim;
using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{

    /// <summary>
    /// Jumps straight to a point in time, skipping any events in-between.
    /// If you want to "fast-forward" time, right now you can set the TimeKeeper's TimeScale
    /// to a high number. Later, I may write a dedicated FF function, but it is not this day.
    /// </summary>
    public class SkipTime : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public int? Month { get; set; }
        public int? Day { get; set; }
        public int? Hour { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            if (Month == null)
            {
                Month = new RPGRef<int>() { ReferenceId = Parameters["Month"] };
            }
            if (Day == null)
            {
                Day = new RPGRef<int>() { ReferenceId = Parameters["Day"] };
            }
            if (Hour == null)
            {
                Hour = new RPGRef<int>() { ReferenceId = Parameters["Hour"] };
            }

            TimeKeeper.Instance.JumpToDateTime(Month.GetValueOrDefault(), Day.GetValueOrDefault(), Hour.GetValueOrDefault());

            completionCallback.Invoke(this);
            yield break;
        }
    }
}