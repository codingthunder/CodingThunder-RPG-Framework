using CodingThunder.RPGUtilities.GameState;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Cmds
{
    public class AutoSave : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
        public object ReturnValue { get; set; }

        public bool Suspended { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            GameRunner.Instance.SaveGame("auto_save");
            yield break;
        }
    }
}