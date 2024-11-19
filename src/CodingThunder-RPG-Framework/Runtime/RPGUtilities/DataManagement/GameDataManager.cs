using CodingThunder.RPGUtilities.SaveData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CodingThunder.RPGUtilities.DataManagement
{
	/// <summary>
	/// TODO: I don't think this should be a Singleton. The only Singleton we probably need is the GameRunner itself.
	/// 
	/// GameDataManager manages persistent non-story data between scenes. It's usually easier to handle primitives and strings
	/// inside of Ink itself, while more complex data objects should be managed here.
	/// There are some rules to how it works that probably need to be defined.
	/// </summary>
	public class GameDataManager : MonoBehaviour
	{
		public static GameDataManager Instance;

		private Dictionary<string, object> GameData = new();

		public readonly string labelRegex = @"\$\$([a-zA-Z0-9\.]+)";

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(this);
				return;
			}
			Instance = this;

			SaveLoad.RegisterSaveLoadCallbacks("GameData", GenerateSaveData, LoadSaveData);

			LookupResolver.Instance.RegisterRootKeyword("GameData", Lookup);
		}


		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		private object GenerateSaveData()
		{
			return GameData;
		}

		private void LoadSaveData(object saveData)
		{
			GameData = (Dictionary<string, object>) saveData;
		}

		public object Lookup(List<string> idChain)
		{
			if (idChain[0] == "GameData")
			{
				idChain.RemoveAt(0);
			}

			//var lookupList = idChain.ToList();

			if (GameData.TryGetValue(idChain[0], out var data))
			{
				idChain.RemoveAt(0);
				return data;
			}

			//while (lookupList.Count > 0)
			//{
			//	var lookupString = string.Join(".", lookupList);

			//	if (GameData.TryGetValue(lookupString, out var data))
			//	{
			//		idChain.RemoveRange(0, lookupList.Count);
			//		return data;
			//	}

			//	lookupList.RemoveAt(lookupList.Count - 1);
			//}

			Debug.Log($"Key {string.Join('.', idChain)} not found in GameData. If data is expected, be sure to set it first. Returning null.");

			//Debug.LogError($"Unable to find key {string.Join('.', idChain)} or any of its subkeys in GameData. GameData itself is not returnable.");
			return null;

		}

		/// <summary>
		/// Will overwrite existing data.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		public void RegisterData(string key, object data)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				Debug.LogError("Cannot Register Data with an empty key.");
			}
			GameData[key] = data;
			Debug.LogWarning($"Registering data to key {key} with data type: {data.GetType().Name}");
		}

		/// <summary>
		/// Removes data from the persisted GameData.
		/// </summary>
		/// <param name="key"></param>
		public void UnregisterData(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				Debug.LogWarning("Removing a GameData key which doesn't exist: " +  key);
				return;
			}

			if (GameData.ContainsKey(key))
			{
				GameData.Remove(key);
			}
		}
	}
}