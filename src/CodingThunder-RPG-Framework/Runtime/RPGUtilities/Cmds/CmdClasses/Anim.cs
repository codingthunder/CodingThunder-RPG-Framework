using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// Used in conjunction with the SpriteAnimator component.
    /// Whatever you're looking for needs to have that component on it.
    /// Aside from Target, all params are optional. Goal is to just be able to control an animator.
    /// If you don't need to adjust a param, then don't. It'll stay the same.
    /// Parameters["Target"] will get you your target.
    /// Parameters["Playback"] to set Playback state. Able to be PLAY, STOP, PAUSE, REWIND. Doesn't support RPG Ref.
    /// Parameters["FPS"] to set FramesPerSecond. We're dealing with sprites, so it makes most sense to me.
    /// Parameters["PlayOnAwake"] for to set if its plays upon awake. Not sure this is even worth adjusting from Ink but oh well.
    /// Parameters["Repeat"] does it keep going?
    /// Parameters["Frame"] sets the frame you want the anim on. Will work even if animation is paused or stopped.
    /// Parameters["AnimKey"] what animation do you want it set to? See SpriteAnimSetSO. Right now, not supporting RPGRef on this one.
    /// </summary>
    public class Anim : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public GameObject Target { get; set; }

        public SpriteAnimPlaybackEnum? Playback { get; set; }
        public float? FPS { get; set; }

        public bool? PlayOnAwake { get; set; }
        public bool? Repeat { get; set; }

        public int? Frame {  get; set; }
        public string AnimKey {  get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            if (Target == null)
            {
                Target = new RPGRef<GameObject>(){ ReferenceId = Parameters["Target"]};
            }

            SpriteAnimator targetAnimator = Target.GetComponent<SpriteAnimator>();

            string paramString = null;

            //Order of AnimKey, Playback, and Frame matter because their operations are dependent upon the previous properties.
            //AnimKey sets the animation. If the Frame is set first and the current animation doesn't have the right number of frames,
            // it'll break.
            //If the Plaback is set to STOP, it'll go to ZEROth frame, which would overwrite the Frame parameter.

            if (AnimKey == null && Parameters.TryGetValue("AnimKey", out paramString))
            {
                AnimKey = paramString;
                targetAnimator.SelectAnim(AnimKey);
            }

            if (Playback == null && Parameters.TryGetValue("Playback", out paramString))
            {
                Playback = (SpriteAnimPlaybackEnum)Enum.Parse(typeof(SpriteAnimPlaybackEnum), paramString, true);
                targetAnimator.Playback = Playback.Value;
            }

            if (Frame == null && Parameters.TryGetValue("Frame", out paramString))
            {
                Frame = new RPGRef<int> { ReferenceId = paramString };
                targetAnimator.SetFrame(Frame.Value);
            }

            // All parameters after this should be able to be set without worrying about order.

            if (FPS == null && Parameters.TryGetValue("FPS", out paramString))
            {
                FPS = new RPGRef<float>() { ReferenceId = paramString };
                targetAnimator.fps = FPS.Value;
            }

            if (PlayOnAwake == null && Parameters.TryGetValue("PlayOnAwake", out paramString))
            {
                PlayOnAwake = new RPGRef<bool>() { ReferenceId = paramString };
                targetAnimator.playOnAwake = PlayOnAwake.Value;
            }

            if (Repeat == null && Parameters.TryGetValue("Repeat", out paramString))
            {
                Repeat = new RPGRef<bool>() { ReferenceId = paramString };
                targetAnimator.repeat = Repeat.Value;
            }

            completionCallback(this);
            yield break;
        }
    }
}