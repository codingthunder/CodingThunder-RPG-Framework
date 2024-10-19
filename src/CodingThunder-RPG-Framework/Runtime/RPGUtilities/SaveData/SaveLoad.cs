using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Linq;

namespace CodingThunder.RPGUtilities.SaveData
{
    public static class SaveLoad
    {
        private static Dictionary<string, Func<object>> saveDataCallbacks = new Dictionary<string, Func<object>>();
        private static Dictionary<string, Action<object>> loadDataCallbacks = new Dictionary<string, Action<object>>();

        public static void RegisterSaveLoadCallbacks(string name, Func<object> saveDataCallback, Action<object> loadDataCallback)
        {
            if (saveDataCallbacks.ContainsKey(name))
            {
                saveDataCallbacks[name] = saveDataCallback;
            }
            else
            {
                saveDataCallbacks.Add(name, saveDataCallback);
            }

            if (loadDataCallbacks.ContainsKey(name))
            {
                loadDataCallbacks[name] = loadDataCallback;
            }
            else
            {
                loadDataCallbacks.Add(name, loadDataCallback);
            }
        }

        public static void DeregisterSaveLoadCallbacks(string name)
        {
            if (saveDataCallbacks.ContainsKey(name)) { saveDataCallbacks.Remove(name); }
            if (loadDataCallbacks.ContainsKey(name)) { loadDataCallbacks.Remove(name); }
        }

        // Saves the game data to a JSON file
        public static void SaveGame(string saveName)
        {
            Dictionary<string, object> saveData = saveDataCallbacks.ToDictionary(x => x.Key, x => x.Value.Invoke());

            string saveDir = Path.Combine(Application.persistentDataPath, "saves");
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            // Include type information in JSON for proper deserialization
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, settings);
            string filePath = Path.Combine(saveDir, saveName + ".json");
            File.WriteAllText(filePath, json);
        }

        // Loads the game data from a JSON file
        public static void LoadGame(string saveName)
        {
            string saveDir = Path.Combine(Application.persistentDataPath, "saves");
            string filePath = Path.Combine(saveDir, saveName + ".json");

            if (!File.Exists(filePath))
            {
                Debug.LogError("Save file not found: " + filePath);
                return;
            }

            string json = File.ReadAllText(filePath);

            // Deserialize the JSON back into an object, preserving type information
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            Dictionary<string, object> saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);

            foreach (var keyPair in saveData)
            {
                if (!loadDataCallbacks.ContainsKey(keyPair.Key))
                {
                    Debug.LogWarning("Tried loading a saveData key without a registered loading Callback. Key: " + keyPair.Key);
                    continue;
                }

                loadDataCallbacks[keyPair.Key].Invoke(keyPair.Value);
            }
        }

        // Lists all available save game names
        public static List<string> ListSaves()
        {
            string saveDir = Path.Combine(Application.persistentDataPath, "saves");
            if (!Directory.Exists(saveDir))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(saveDir, "*.json");
            var saveNames = new List<string>();

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                saveNames.Add(fileName);
            }

            return saveNames;
        }
    }
}
