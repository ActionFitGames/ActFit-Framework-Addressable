
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// Generates JSON files mapping addressable asset keys to enum-compatible keys.
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
        /// and saving it as JSON files.
        /// </summary>
        protected override void GenerateProcess()
        {
            var assetEntries = GetAddressableAssetEntries();
            var assetStringData = GenerateDataMappingData(assetEntries);
            var assetIntegerData = GenerateDataMappingIntegerData(assetEntries);

            if (assetStringData == null || assetIntegerData == null)
            {
                Debug.LogError("Addressable Data Map is null. Check AddressableGroup.");
                return;
            }

            EditorExtensions.EnsureDirectoryExists(AssetPath);

            // Save string mapping data to JSON
            var saveStringPath = Path.Combine(AssetPath, "JsonKVPData.json");
            var jsonStringData = JsonConvert.SerializeObject(assetStringData, Formatting.Indented);
            File.WriteAllText(saveStringPath, jsonStringData);
            Debug.Log($"Addressables String JSON saved to {saveStringPath}");

            // Save integer mapping data to JSON
            var saveIntegerPath = Path.Combine(AssetPath, "JsonKVPIntegerData.json");
            var jsonIntegerData = JsonConvert.SerializeObject(assetIntegerData, Formatting.Indented);
            File.WriteAllText(saveIntegerPath, jsonIntegerData);
            Debug.Log($"Addressables Integer JSON saved to {saveIntegerPath}");

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

        /// <summary>
        /// Generates a dictionary that maps addressable asset keys to integer values starting from 1000001.
        /// The keys are derived from the asset type and primary key.
        /// </summary>
        /// <param name="assetEntries">A list of AddressableAssetEntry objects to process.</param>
        /// <returns>A dictionary where the keys are asset identifiers and the values are integers.</returns>
        private Dictionary<string, int> GenerateDataMappingIntegerData(List<AddressableAssetEntry> assetEntries)
        {
            var mappingData = new Dictionary<string, int>();
            int currentIndex = 1000001;

            foreach (var assetEntry in assetEntries)
            {
                var assetTypeString = assetEntry.MainAssetType.ToString();
                var assetPrimaryKey = assetEntry.address;

                var mappingKey = GenerateMappingEnumKey(assetTypeString, assetPrimaryKey);

                if (!mappingData.ContainsKey(mappingKey))
                {
                    mappingData[mappingKey] = currentIndex++;
                }
            }

            return mappingData;
        }
    }
}