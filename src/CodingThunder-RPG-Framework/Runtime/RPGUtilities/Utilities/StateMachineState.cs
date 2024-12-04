using System;

namespace CodingThunder.RPGUtilities.Utilities
{
    /// <summary>
    /// If there's a StateMachineState, shouldn't there also be a StateMachine?
    /// You'd think so, but that'll come later because JC is lazy.
    /// The purpose of this is just an easy way to easily call certain functions upon entering, maintaining, and exiting a certain state.
    /// Where and how you call those functions is up to you. Because you're passing in an Action, you can have those Actions
    /// reference member variables in your classes.
    /// </summary>
    public class StateMachineState
    {
        public string StateName { get; private set; }
        private Action onEnterState;
        private Action onExitState;
        private Action onExecuteState;
        public StateMachineState(string stateName, Action enterStateCallback, Action executeStateCallback, Action exitStateCallback) {
            StateName = stateName;
            onEnterState = enterStateCallback;
            onExitState = exitStateCallback;
            onExecuteState = executeStateCallback;
        }

        public void EnterState()
        {
            onEnterState();
        }

        public void ExitState()
        {
            onExitState();
        }

        public void ExecuteState()
        {
            onExecuteState();
        }
    }
}