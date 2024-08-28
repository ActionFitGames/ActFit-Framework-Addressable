using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Abstract base class for generating addressable-related assets and data. 
    /// Provides methods for processing and handling Addressable assets, ensuring proper setup and generation of required data.
    /// </summary>
    public abstract class AbstractGenerator
    {
        #region Fields

        protected static AddressableAssetSettings Settings;
        
        private const string EnumKeyGenerationFlag = "EnumKey_Generation_Flag";
        private const string ConfigGenerationFlag = "Config_Generation_Flag";

        private bool _isProcessingComplete = false;
        
        #endregion

        #region Public Access

        /// <summary>
        /// Initiates the generation process. Optionally enables the use of actual keys as enum symbols.
        /// Refreshes the AssetDatabase after generation.
        /// </summary>
        /// <param name="isEnumDefineSymbol">If true, enables actual keys as enum symbols.</param>
        protected void Generate(bool isEnumDefineSymbol = false)
        {
            if (!GetAddressableSetting())
            {
                return;
            }
            
            EditorApplication.update += OnEditorUpdate;

            GenerateProcess();
            return;

            void OnEditorUpdate()
            {
                if (_isProcessingComplete)
                {
                    if (isEnumDefineSymbol)
                    {
                        AddressableDefineSymbols.EnableUseActualKey();
                    }
                
                    AssetDatabase.Refresh();
                    EditorApplication.update -= OnEditorUpdate;
                }
            }
        }

        protected abstract void GenerateProcess();

        protected void MarkProcessingComplete()
        {
            _isProcessingComplete = true;
        }

        #endregion

        #region Protected Methods (Addressable Associated)

        /// <summary>
        /// Retrieves a dictionary mapping internal IDs to addressable data from a list of AddressableAssetEntry objects.
        /// Ensures that each entry is unique and processes valid locations for each asset.
        /// </summary>
        /// <param name="assetEntries">List of AddressableAssetEntry objects.</param>
        /// <returns>Dictionary mapping internal IDs to addressable data, or null if an error occurs.</returns>
        protected static async UniTask<Dictionary<string, string>> GetAddressableDataMapAsync(List<AddressableAssetEntry> assetEntries)
        {
            var addressableDataMap = new Dictionary<string, string>();

            foreach (var entry in assetEntries)
            {
                var resourceLocations = await Addressables.LoadResourceLocationsAsync(entry.address).ToUniTask();

                if (resourceLocations == null)
                {
                    return null;
                }

                foreach (var location in resourceLocations)
                {
                    Debug.Log(location.InternalId);
                    var internalID = location.InternalId;
                    var fileName = ConvertFileName(Path.GetFileName(internalID));

                    if (string.IsNullOrEmpty(internalID))
                    {
                        continue;
                    }
                    
                    addressableDataMap.TryAdd(internalID, fileName);
                }
            }

            foreach (var kvp in addressableDataMap)
            {
                Debug.Log(kvp.Key + " , " + kvp.Value);
            }

            return addressableDataMap;
        }

        /// <summary>
        /// Retrieves a list of AddressableAssetEntry objects from all Addressable groups in the project.
        /// Only entries with labels are included.
        /// </summary>
        /// <returns>List of AddressableAssetEntry objects.</returns>
        protected static List<AddressableAssetEntry> GetAddressableAssetEntries()
        {
            var addressableEntries = new List<AddressableAssetEntry>();
            foreach (var group in Settings.groups)
            {
                if (!group)
                {
                    continue;
                }

                foreach (var entry in group.entries)
                {
                    // 레이블이 없는 엔트리는 제외
                    if (entry.labels == null || entry.labels.Count == 0)
                    {
                        continue;
                    }

                    addressableEntries.Add(entry);
                }
            }

            return addressableEntries;
        }

        #endregion

        #region Protected Methods (Get And Utils)

        /// <summary>
        /// Retrieves the current AddressableAssetSettings object.
        /// Logs an error if the settings cannot be found.
        /// </summary>
        /// <returns>True if settings are found; otherwise, false.</returns>
        private static bool GetAddressableSetting()
        {
            Settings = AddressableAssetSettingsDefaultObject.Settings;

            if (Settings != null)
            {
                return true;
            }
            
            Debug.LogError("AddressableAssetSettings not found. Please ensure 'Setup'.");
            return false;
        }
        
        /// <summary>
        /// Ensures that a specified directory exists, creating it if necessary.
        /// Logs the creation of the directory.
        /// </summary>
        /// <param name="path">The path to the directory to ensure exists.</param>
        protected static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created Directory: {path}");
            }
        }
        
        /// <summary>
        /// Converts a file name into a valid enum name by removing invalid characters
        /// and ensuring it follows a valid enum naming convention.
        /// </summary>
        /// <param name="fileName">The original file name.</param>
        /// <returns>A string representing the converted enum name.</returns>
        protected static string ConvertFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();

            string enumName = string.Empty;

            foreach (char c in fileNameWithoutExtension)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    enumName += c;
                }
                else if (c == '.')
                {
                    enumName += '_';
                }
            }

            if (enumName.Length > 0 && char.IsDigit(enumName[0]))
            {
                enumName = "_" + enumName;
            }

            if (!string.IsNullOrEmpty(extension))
            {
                var extensionCamelCase = char.ToUpper(extension[0]) + extension.Substring(1);
                enumName += $"_{extensionCamelCase}";
            }

            return enumName;
        }

        #endregion
    }
}