
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// Generates a JSON file mapping addressable asset keys to enum-compatible keys.
    /// This generator processes all labeled addressable assets and saves the mapping in a specified directory.
    /// </summary>
    public sealed class AddressableJsonMappingGenerator : AbstractGenerator
    {
        protected override string AssetPath { get; set; } = RootAssetPath + "Data/Addressables/";

        /// <summary>
        /// Generates a key-value mapping data file in JSON format for all addressable assets.
        /// This method is exposed as a Unity editor menu item.
        /// </summary>
        [MenuItem("ActFit/Addressables/[Generate] Key Value Data with JSON (1-Step)", priority = 1)]
        public static void GenerateJsonKeyValueData()
        {
            new AddressableJsonMappingGenerator().Generate();
        }
        
        /// <summary>
        /// Executes the process of generating the addressable asset mapping data
        /// and saving it as a JSON file.
        /// </summary>
        protected override void GenerateProcess()
        {
            var assetEntries = GetAddressableAssetEntries();
            var assetData = GenerateDataMappingData(assetEntries);

            if (assetData == null)
            {
                Debug.LogError("Addressable Data Map is null. Check AddressableGroup.");
                return;
            }

            EditorExtensions.EnsureDirectoryExists(AssetPath);

            var savePath = Path.Combine(AssetPath, "JsonKVPData.json");
            var jsonString = JsonConvert.SerializeObject(assetData, Formatting.Indented);
            File.WriteAllText(savePath, jsonString);
            
            Debug.Log($"Addressables JSON saved to {savePath}");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Generates a dictionary that maps addressable asset keys to enum-compatible keys.
        /// The keys are derived from the asset type and primary key.
        /// </summary>
        /// <param name="assetEntries">A list of AddressableAssetEntry objects to process.</param>
        /// <returns>A dictionary where the keys are asset identifiers and the values are enum-compatible keys.</returns>
        private Dictionary<string, string> GenerateDataMappingData(List<AddressableAssetEntry> assetEntries)
        {
            var mappingData = new Dictionary<string, string>();

            foreach (var assetEntry in assetEntries)
            {
                var assetTypeString = assetEntry.MainAssetType.ToString();
                var assetPrimaryKey = assetEntry.address;

                var mappingKey = assetTypeString + assetPrimaryKey;
                var mappingEnumKey = GenerateMappingEnumKey(assetTypeString, assetPrimaryKey);

                mappingData.TryAdd(mappingKey, mappingEnumKey);
            }

            return mappingData;
        }
    }
}