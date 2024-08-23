
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// The AddressablePairFactory class is responsible for loading and parsing a JSON file that maps internal IDs to AddressableKey enum values.
    /// It provides a method to retrieve this mapping as a dictionary, ensuring that the JSON file exists and handling potential parsing errors.
    /// </summary>
    public static class AddressablePairFactory
    {
        private const string JsonAssetPath = "Assets/Game/Data/Addressables/AddressableKeyValueData.json";

        public static Dictionary<string, AddressableKey> GetAddressableKeysMap()
        {
            if (!File.Exists(JsonAssetPath))
            {
                Debug.LogError("KeyValueData file not found. Ensure the JSON file exists at the specified path.");
                return null;
            }
            
            var jsonData = File.ReadAllText(JsonAssetPath);
            var addressableDataMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            var addressableKeysMap = new Dictionary<string, AddressableKey>();
            foreach (var kvp in addressableDataMap)
            {
                string internalId = kvp.Key;
                string enumKeyString = kvp.Value;

                // enumKey를 실제 AddressableEnumKey로 변환
                if (System.Enum.TryParse(enumKeyString, out AddressableKey enumKey))
                {
                    addressableKeysMap[internalId] = enumKey;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse enum key '{enumKeyString}' for InternalId '{internalId}'.");
                }
            }

            return addressableKeysMap;
        }
    }
}