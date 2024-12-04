using CodingThunder.RPGUtilities.GameState;
using CodingThunder.RPGUtilities.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Mechanics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : GameStateManaged
    {
        [SerializeField]
        public SpriteAnimPlaybackEnum _playback = SpriteAnimPlaybackEnum.STOP;

        public SpriteAnimPlaybackEnum Playback
        {
            get { return _playback; }
            set { _playback = value; ChangeAnimatorState(_playback); }
        }

        public bool playOnAwake = false;
        public bool repeat = false;
        public float fps = 12f;

        public int currentFrame = 0;
        public string currentAnimKey = "default";

        public SpriteAnimSetSO spriteAnimSet;

        private SpriteRenderer spriteRenderer;
        private SpriteAnim currentAnimation;

        private float timeSinceLastAnimFrame;

        private StateMachineState currentState;

        private StateMachineState playState;
        private StateMachineState stopState;
        private StateMachineState pauseState;
        private StateMachineState rewindState;

        protected override void OnAwake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentAnimation = spriteAnimSet.GetAnimByKey(currentAnimKey);

            spriteRenderer.sprite = currentAnimation.sprites[0];

            playState = new StateMachineState(
                SpriteAnimPlaybackEnum.PLAY.ToString(),
                () => { return; },
                ExecutePlayState,
                () => { return; }
            );

            stopState = new StateMachineState(
                SpriteAnimPlaybackEnum.STOP.ToString(),
                EnterStopState,
                () => { return; },
                () => { return; }
            );

            pauseState = new StateMachineState(
                SpriteAnimPlaybackEnum.PAUSE.ToString(),
                () => { return; },
                () => { return; },
                () => { return; }
            );

            rewindState = new StateMachineState(
                SpriteAnimPlaybackEnum.REWIND.ToString(),
                () => { return; },
                ExecuteRewindState,
                () => { return; }
            );

            if (playOnAwake)
            {
               Playback = SpriteAnimPlaybackEnum.PLAY;
               return;
            }

            ChangeAnimatorState(_playback);
        }

        protected override void OnUpdate()
        {
            if (!IsActive)
            {
                return;
            }
            if (currentAnimKey != currentAnimation.animName)
            {
                SelectAnim(currentAnimKey);
            }

            // This is an absolutely atrocious way of doing it, but I'm feeling lazy.
            // This way, if the user changes the Playback in the editor, it'll show in the game.
            if (currentState.StateName != Playback.ToString())
            {
                ChangeAnimatorState(Playback);
            }

            currentState.ExecuteState();
        }



        public void SelectAnim(string animKey)
        {
            if (currentAnimKey != animKey)
            {
                currentAnimKey = animKey;
            }

            currentAnimation = spriteAnimSet.GetAnimByKey(currentAnimKey);

            spriteRenderer.sprite = currentAnimation.sprites[0];
            currentFrame = 0;
        }

        public void SetFrame(int frame)
        {
            currentFrame = frame;

            spriteRenderer.sprite = currentAnimation.sprites[currentFrame];
        }

        public void NextFrame()
        {
            if (currentFrame < 0)
            {
                currentFrame = 0;
            }
            currentFrame++;

            if (currentFrame > currentAnimation.sprites.Count - 1)
            {
                if (!repeat)
                {
                    return;
                }
                currentFrame = 0;
            }

            spriteRenderer.sprite = currentAnimation.sprites[currentFrame];
        }

        public void PrevFrame()
        {
            if (currentFrame > currentAnimation.sprites.Count - 1)
            {
                currentFrame = currentAnimation.sprites.Count - 1;
            }

            currentFrame--;
            if (currentFrame < 0)
            {
                if (!repeat)
                {
                    return;
                }
                currentFrame = currentAnimation.sprites.Count - 1;
            }

            spriteRenderer.sprite = currentAnimation.sprites[currentFrame];
        }

        #region EXTERNAL_CALLS

        public void PlayAnim(float? fps = null)
        {

        }

        public void StopAnim()
        {

        }

        public void PauseAnim()
        {

        }

        public void RewindAnim(float? fps = null)
        {

        }

        #endregion

        private void ChangeAnimatorState(SpriteAnimPlaybackEnum state)
        {
            currentState?.ExitState();

            switch (state)
            {
                case SpriteAnimPlaybackEnum.STOP:
                    currentState = stopState; break;
                case SpriteAnimPlaybackEnum.PLAY:
                    currentState = playState; break;
                case SpriteAnimPlaybackEnum.PAUSE:
                    currentState = pauseState; break;
                case SpriteAnimPlaybackEnum.REWIND:
                    currentState = rewindState; break;
            }

            currentState.EnterState();
        }

        private void EnterStopState()
        {
            timeSinceLastAnimFrame = 0f;
            currentFrame = 0;
        }
        private void ExecutePlayState()
        {
            if (fps == 0f)
            {
                NextFrame();
                return;
            }

            timeSinceLastAnimFrame += Time.deltaTime;

            if (timeSinceLastAnimFrame > 1.0 / fps)
            {
                timeSinceLastAnimFrame = 0;
                NextFrame();
            }
        }

        private void ExecuteRewindState()
        {
            if (fps == 0f)
            {
                PrevFrame();
                return;
            }

            timeSinceLastAnimFrame -= Time.deltaTime;

            if (timeSinceLastAnimFrame <= 0)
            {
                timeSinceLastAnimFrame = 1.0f / fps;
                PrevFrame();
            }
        }
    }



    public enum SpriteAnimPlaybackEnum
    {
        STOP,
        PLAY,
        PAUSE,
        REWIND
    }
}