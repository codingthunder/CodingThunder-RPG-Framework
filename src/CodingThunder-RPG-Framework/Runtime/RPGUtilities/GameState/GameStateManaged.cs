using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.GameState
{
    public abstract class GameStateManaged : MonoBehaviour
    {
        public List<GameStateEnum> whitelistedStates = new List<GameStateEnum>();

        public bool IsActive { get; private set; }

        private void Awake()
        {
            GameRunner.Instance.OnChangeGameState += OnGameStateChange;
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

            OnGameStateChange(GameRunner.Instance.GameState);
            if (!IsActive)
            {
                return;
            }

            OnStart();
        }

        /// <summary>
        /// OnStart will be called BEFORE StateChange is applied.
        /// </summary>
        protected virtual void OnStart() { }

        // Update is called once per frame
        void Update()
        {
            if (!IsActive) return;
            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        private void FixedUpdate()
        {
            if (!IsActive) return;
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate() { }

        private void OnDestroy()
        {
            GameRunner.Instance.OnChangeGameState -= OnGameStateChange;
            HandleDestroy();
        }

        protected virtual void HandleDestroy()
        {

        }

        private void OnGameStateChange(GameStateEnum state)
        {
            IsActive = whitelistedStates.Contains(state);

            Debug.Log($"{gameObject.name} has received GameState {state} and IsActive: {IsActive}");

            HandleGameStateChange(state);
        }

        protected virtual void HandleGameStateChange(GameStateEnum state)
        {

        }


    }
}