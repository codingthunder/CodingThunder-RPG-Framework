using System.Collections;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Triggers
{
    public class TimerTrigger : InteractTrigger
    {
        public bool startOnAwake = true;

        public float countdown = 0;

        public bool repeat = false;

        private bool triggered;

        protected override void OnAwake()
        {
            if (startOnAwake && !triggered)
            {
                StartCoroutine(Countdown());
            }
        }

        public void TriggerCountdown()
        {
            if (!IsActive || triggered)
            {
                return;
            }

            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            triggered = true;
            float timeLeft = countdown;

            while (timeLeft > 0)
            {
                yield return null;
                if (!IsActive)
                {
                    continue;
                }

                timeLeft -= Time.deltaTime;
            }

            RunTrigger();

            if (repeat)
            {
                triggered = false;
                StartCoroutine(Countdown());
            }
        }
    }
}