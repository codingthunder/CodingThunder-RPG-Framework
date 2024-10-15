using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.GameState
{

	/// <summary>
	/// Needs to be singleton. It's Highlander. THERE CAN BE ONLY ONE.
	/// Responsible for connecting together all the different pieces.
	/// </summary>
	/// 
	[RequireComponent(typeof(StoryRunner))]
	[RequireComponent(typeof(GameDataManager))]
	[RequireComponent(typeof(SceneDataManager))]

	public class GameRunner : MonoBehaviour
	{
		public static GameRunner Instance { get; private set; }

		private GameDataManager gameDataManager;
		private SceneDataManager sceneDataManager;
		private StoryRunner storyRunner;


		[SerializeField]
		private string skipToSceneOnStart;
		public GameStateEnum GameState { get; private set; }
		public event Action<GameStateEnum> OnChangeGameState;

		[Header("Use this to register all the classes in a namespace to the Expression Evaluator.")]
		[SerializeField]
		private List<string> registeredNamespaces = new List<string>();
		[Header("Use this to register all a single in a class (with its fully qualified namespace) to the Expression Evaluator.")]
		[SerializeField]
		private List<string> registeredClasses = new List<string>();

		private void ChangeGameState(GameStateEnum gameState)
		{
			GameState = gameState;
		}

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(this);
			}
			Instance = this;

			DontDestroyOnLoad(gameObject);

			gameDataManager = GetComponent<GameDataManager>();
			sceneDataManager = GetComponent<SceneDataManager>();
			storyRunner = GetComponent<StoryRunner>();

			OnChangeGameState += ChangeGameState;

			RegisterNamespacesAndClasses();
		}

		private void RegisterNamespacesAndClasses()
		{
			foreach(var ns in registeredNamespaces)
			{
				DynamicExpressoEvaluator.Instance.RegisterNamespace(ns);
			}

			foreach( var cls in registeredClasses)
			{
				DynamicExpressoEvaluator.Instance.RegisterTypeIfNotRegistered(Type.GetType(cls));
			}
		}

		// Start is called before the first frame update
		void Start()
		{
			storyRunner.onSceneEnd += ResumePlayFromCutscene;


			OnChangeGameState.Invoke(GameStateEnum.PAUSED);

			if (!string.IsNullOrWhiteSpace(skipToSceneOnStart))
			{
				StartCutscene(skipToSceneOnStart);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void StartCutscene(string cutsceneId)
		{
			OnChangeGameState?.Invoke(GameStateEnum.CUTSCENE);
			storyRunner.GoToChapter(cutsceneId);
		}

		private void ResumePlayFromCutscene()
		{
			
			Debug.Log("Hey, you should be resuming play now.");
			UnpauseGame();
		}

		public void NewGame()
		{
			OnChangeGameState?.Invoke(GameStateEnum.CUTSCENE);
			storyRunner.NewStory();
		}

		public void LoadGame(string saveName)
		{
			OnChangeGameState?.Invoke(GameStateEnum.LOADING);
			SaveLoad.LoadGame(saveName);
			OnChangeGameState?.Invoke(GameStateEnum.CUTSCENE);
			storyRunner.Next();
		}

		public void SaveGame(string saveName)
		{
            OnChangeGameState?.Invoke(GameStateEnum.LOADING);
            SaveLoad.SaveGame(saveName);
            OnChangeGameState?.Invoke(GameStateEnum.CUTSCENE);
            storyRunner.Next();
        }

		public void PauseGame()
		{
			OnChangeGameState?.Invoke(GameStateEnum.PAUSED);
		}

		public void UnpauseGame()
		{
			OnChangeGameState?.Invoke(GameStateEnum.PLAY);
		}

		private void OnDestroy()
		{
			OnChangeGameState -= ChangeGameState;
		}
	}
}