using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.SaveData;
using CodingThunder.RPGUtilities.RPGStory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodingThunder.RPGUtilities.Cmds;

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

		[HideInInspector]
		public GameDataManager gameDataManager;
		[HideInInspector]
		public SceneDataManager sceneDataManager;
		[HideInInspector]
		public StoryRunner storyRunner;

		public bool debugMode;

		[SerializeField]
		[Header("Will skip game load.")]
		private string skipToSceneOnStart;
		[field: SerializeField]
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
			OnChangeGameState?.Invoke(GameState);
		}

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;

			// DontDestroyOnLoad(gameObject);

			//Instantiating a CmdExpression during gameplay will cause the Cmd Lookup Dictionary to get filled.
			//Figure doing it here guarantees it won't happen somewhere else when I don't want it to.
			var junkExpression = new CmdExpression();


			if (gameDataManager == null) gameDataManager = GetComponent<GameDataManager>();
			if (sceneDataManager == null) sceneDataManager = GetComponent<SceneDataManager>();
			if (storyRunner == null) storyRunner = GetComponent<StoryRunner>();

			sceneDataManager.RegisterDontDestroyOnLoad(gameObject);

			storyRunner.RegisterCutsceneTriggerCallback(NowInACutsceneState);

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


			ChangeGameState(GameState);

			if (!string.IsNullOrWhiteSpace(skipToSceneOnStart))
			{
				StartStoryFlow(skipToSceneOnStart);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void StartStoryFlow(string cutsceneId)
		{
			//ChangeGameState(GameStateEnum.CUTSCENE);
			storyRunner.GoToChapter(cutsceneId);
		}


		/// <summary>
		/// Meant to pretty much only be used by the StoryRunner as a callback.
		/// There IS a better way of doing this, but I'm lazy.
		/// </summary>
		private void NowInACutsceneState()
		{
			if (GameState == GameStateEnum.CUTSCENE)
			{
				return;
			}
			ChangeGameState(GameStateEnum.CUTSCENE);
		}

		private void ResumePlayFromCutscene()
		{
			
			Debug.Log("Hey, you should be resuming play now.");
			UnpauseGame();
		}

		public void NewGame()
		{
			ChangeGameState(GameStateEnum.CUTSCENE);
			storyRunner.NewStory();
		}

		public void LoadGame(string saveName)
		{
			ChangeGameState(GameStateEnum.LOADING);
			SaveLoad.LoadGame(saveName);
			//ChangeGameState(GameState);
			//storyRunner.Next();
		}

		public void SaveGame(string saveName)
		{
            SaveLoad.SaveGame(saveName);
        }

		public void PauseGame()
		{
			ChangeGameState(GameStateEnum.PAUSED);
		}

		public void UnpauseGame()
		{
			ChangeGameState(GameStateEnum.PLAY);
		}

		private void OnDestroy()
		{
			if (Instance != this)
			{
				return;
			}
			sceneDataManager.DeregisterDontDestroyOnLoad(gameObject);
			SaveLoad.DeregisterSaveLoadCallbacks("Metadata");
		}
	}
}