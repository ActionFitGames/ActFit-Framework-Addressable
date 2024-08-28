
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// Abstract base class for generating addressable-related assets and data. 
    /// Provides methods for processing and handling Addressable assets, ensuring proper setup and generation of required data.
    /// </summary>
    public abstract class AbstractGenerator
    {
        #region Fields

        protected static AddressableAssetSettings Settings;

        protected const string RootAssetPath = "Assets/Games/";
        protected const string JsonAssetPath = RootAssetPath + "Data/Addressables/JsonKVPData.json";
        protected virtual string AssetPath { get; set; }
        
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

            GenerateProcess();

            if (isEnumDefineSymbol)
            {
                AddressableDefineSymbols.EnableUseActualKey();
            }
            
            AssetDatabase.Refresh();
        }

        protected abstract void GenerateProcess();

        #endregion

        #region Protected Methods (Addressable Associated)

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

            if (Settings)
            {
                return true;
            }
            
            Debug.LogError("AddressableAssetSettings not found. Please ensure 'Setup'.");
            return false;
        }

        /// <summary>
        /// Generates a valid enum key by combining the asset type and a sanitized version of the asset's file name and extension.
        /// This method removes namespaces from the asset type and replaces special characters in the file name with underscores.
        /// If the resource type contains special characters (such as TMP_Font), underscores after the namespace separator are removed.
        /// The file extension is treated separately, with each part converted to CamelCase and separated by underscores.
        /// </summary>
        /// <param name="assetTypeString">The type of the asset, typically includes the namespace (e.g., UnityEngine.Object).</param>
        /// <param name="addressKey">The address or path of the asset, used to extract the file name and extension.</param>
        /// <returns>A sanitized string that can be used as an enum key, combining the asset type, file name, and file extension.</returns>
        protected static string GenerateMappingEnumKey(string assetTypeString, string addressKey)
        {
            // 네임스페이스 제거 및 언더바 이후 부분 처리
            if (assetTypeString.Contains("."))
            {
                var lastDotIndex = assetTypeString.LastIndexOf('.');
                assetTypeString = assetTypeString.Substring(lastDotIndex + 1);
            }

            // 리소스 타입에서 언더바 제거
            assetTypeString = RemoveSpecialCharacters(assetTypeString);

            // 파일 이름과 확장자 추출
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(addressKey);
            var fileExtension = Path.GetExtension(addressKey).TrimStart('.').ToLowerInvariant();

            // 유효한 Enum 이름으로 변환 (특수문자 제거)
            var validFileName = RemoveSpecialCharacters(fileNameWithoutExtension);
            var validFileExtension = ConvertToCamelCase(fileExtension);

            // 최종 EnumKey 생성 (예: Object_BlahBlah_Prefab)
            var enumKey = $"{assetTypeString}_{validFileName}_{validFileExtension}";

            return enumKey;
        }

        /// <summary>
        /// Removes special characters from the file name or resource type, ensuring that the result is a valid enum name.
        /// This method replaces non-alphanumeric characters with an empty string, effectively removing them.
        /// </summary>
        /// <param name="name">The original string to be converted.</param>
        /// <returns>A sanitized string where special characters are removed.</returns>
        private static string RemoveSpecialCharacters(string name)
        {
            var enumName = string.Empty;

            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c))
                {
                    enumName += c;
                }
                // 특수 문자를 제거하기 위해 아무 처리도 하지 않음
            }

            return enumName;
        }

        /// <summary>
        /// Converts a string (such as a file extension) into CamelCase format, separating parts with underscores.
        /// </summary>
        /// <param name="name">The original string to be converted.</param>
        /// <returns>A string converted to CamelCase with parts separated by underscores.</returns>
        private static string ConvertToCamelCase(string name)
        {
            var enumName = string.Empty;
            var capitalizeNext = true;

            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c))
                {
                    enumName += capitalizeNext ? char.ToUpper(c) : c;
                    capitalizeNext = false;
                }
                else
                {
                    capitalizeNext = true; // 다음 문자는 대문자로 변환
                    enumName += "_"; // 특수 문자는 _로 대체
                }
            }

            return enumName;
        }
        
        /// <summary>
        /// Converts a file name into a valid enum name by removing invalid characters
        /// and ensuring it follows a valid enum naming convention.
        /// </summary>
        /// <param name="fileName">The original file name.</param>
        /// <returns>A string representing the converted enum name.</returns>
        [Obsolete("Don't use this, use the GenerateMappingEnumKey")]
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