using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.SaveData;
using CodingThunder.RPGUtilities.RPGStory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

		public GameDataManager gameDataManager;
		public SceneDataManager sceneDataManager;
		public StoryRunner storyRunner;

		public bool debugMode;

		[SerializeField]
		[Header("Will skip game load.")]
		private string skipToSceneOnStart;
		public GameStateEnum GameState { get; private set; }
		public event Action<GameStateEnum> OnChangeGameState;

		[Header("Use this to register all the classes in a namespace to the Expression Evaluator.")]
		[SerializeField]
		private List<string> registeredNamespaces = new List<string>();
		[Header("Use this to register a single class (with its fully qualified namespace) to the Expression Evaluator.")]
		[SerializeField]
		private List<string> registeredClasses = new List<string>();

		private void ChangeGameState(GameStateEnum gameState)
		{
			if (debugMode)
			{
				UnityEngine.Debug.Log($"Changing GameState to: {GameState}");
			}
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

			if (gameDataManager == null) gameDataManager = GetComponent<GameDataManager>();
			if (sceneDataManager == null) sceneDataManager = GetComponent<SceneDataManager>();
			if (storyRunner == null) storyRunner = GetComponent<StoryRunner>();

			OnChangeGameState += ChangeGameState;

			SaveLoad.RegisterSaveLoadCallbacks("Metadata", SaveGameMetadata, LoadGameMetadata);

			RegisterNamespacesAndClasses();
		}

		private object SaveGameMetadata()
		{
			Dictionary<string, object> metadata = new()
            {
                { "GameState", GameState },
                { "GameScene", SceneManager.GetActiveScene().name }
            };

			return metadata;
		}


		private void LoadGameMetadata(object data)
		{
			var metadata = data as Dictionary<string, object>;
			int gameStateInt = Convert.ToInt32( metadata["GameState"]);
			GameState = (GameStateEnum)gameStateInt;
			//GameState = (GameStateEnum) (int) metadata["GameState"];
			var sceneName = (string)metadata["GameScene"];

            SceneManager.sceneLoaded += OnSceneLoadFromMetadata;

            SceneManager.LoadScene(sceneName);
		}

        //This is structured so that it should only ever be called if a scene is loaded as part of Loading a Game from disk.
        //In most cases, handing off control to the storyRunner is managed elsewhere. But in this case, it is necessary to be explicit here.
        private void OnSceneLoadFromMetadata(Scene scene, LoadSceneMode loadSceneMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadFromMetadata;
            if (GameState == GameStateEnum.CUTSCENE)
            {
                storyRunner.Next();
            }
            //Uhhh... handle otherCollider states here as necessary. 

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
			//OnChangeGameState?.Invoke(GameState);
			//storyRunner.Next();
		}

		public void SaveGame(string saveName)
		{
            SaveLoad.SaveGame(saveName);
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
			SaveLoad.DeregisterSaveLoadCallbacks("Metadata");
		}
	}
}