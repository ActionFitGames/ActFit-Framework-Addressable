
using System;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// A generator class that creates and updates Addressable Cache Scriptable Objects based on the Addressable system configuration.
    /// </summary>
    public sealed class AddressableCacheGenerator : AbstractGenerator
    {
        protected override string AssetPath { get; set; } = "Assets/Resources/ScriptableObjects/Addressables/";

        /// <summary>
        /// Generates the Addressable Cache Scriptable Object (SO) by running the 3-step process.
        /// This is accessible from the Unity Editor menu.
        /// </summary>
        [MenuItem("ActFit/Addressables/[Generate] Addressable CacheSO (3-Step)", priority = 3)]
        public static void GenerateAddressableCacheSO()
        {
            new AddressableCacheGenerator().Generate();
        }
        
        /// <summary>
        /// The core process that handles the generation of the Addressable Cache Scriptable Object.
        /// </summary>
        protected override void GenerateProcess()
        {
            EditorExtensions.EnsureDirectoryExists(AssetPath);

            var cacheSO = CreateOrGetAddressableCacheSO();
            
            cacheSO.LabelStrings = GetAllLabelStrings();
            cacheSO.AssetKeysMap = GetKeysMappingCache();

            EditorUtility.SetDirty(cacheSO);
            AssetDatabase.SaveAssets();
            
            Debug.Log("[Cache] Addressable Cache Config is generated. (Complete!)");
        }

        #region Internal Methods

        /// <summary>
        /// Creates a new AddressableCacheSO if one does not already exist, or retrieves the existing one.
        /// </summary>
        /// <returns>The existing or newly created AddressableCacheSO.</returns>
        private AddressableCacheSO CreateOrGetAddressableCacheSO()
        {
            var path = $"{AssetPath}AddressableCache.asset";
            var cacheSO = AssetDatabase.LoadAssetAtPath<AddressableCacheSO>(path);

            if (!cacheSO)
            {
                cacheSO = ScriptableObject.CreateInstance<AddressableCacheSO>();
                AssetDatabase.CreateAsset(cacheSO, path);
                Debug.Log($"Created AddressableCacheSO at {path}");
            }
            else
            {
                Debug.Log("AddressableCacheSO already exists and will be [Update]");
            }

            return cacheSO;
        }

        /// <summary>
        /// Loads the keys and their associated AddressableKey enums from a JSON file and returns a SerializedDictionary.
        /// </summary>
        /// <returns>A SerializedDictionary mapping primary keys to AddressableKey enums.</returns>
        private SerializedDictionary<string, AddressableKey> GetKeysMappingCache()
        {
            if (!File.Exists(JsonAssetPath))
            {
                Debug.LogError("KeyValueData file not found. Ensure the JSON file exists at the specified path.");
                return null;
            }

            var jsonData = File.ReadAllText(JsonAssetPath);
            var entryDataMap = JsonConvert.DeserializeObject<SerializedDictionary<string, string>>(jsonData);
            var newKvp = new SerializedDictionary<string, AddressableKey>();

            foreach (var kvp in entryDataMap)
            {
                var primaryKey = kvp.Key;
                var enumValue = kvp.Value;

                if (Enum.TryParse(enumValue, out AddressableKey addressableKey))
                {
                    newKvp[primaryKey] = addressableKey;
                }
                else
                {
                    Debug.LogError($"Failed to parse enum key '{enumValue}' for PrimaryKey '{primaryKey}'.");
                    return null;
                }
            }

            return newKvp;
        }

        /// <summary>
        /// Retrieves all label strings from the Addressable system settings.
        /// </summary>
        /// <returns>A list of label strings used in the Addressable system.</returns>
        private List<string> GetAllLabelStrings()
        {
            return Settings.GetLabels();
        }

        #endregion
    }
}