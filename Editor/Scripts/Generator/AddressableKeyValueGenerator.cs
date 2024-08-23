
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// The AddressableKeyValueGenerator class generates a JSON file mapping Addressable internal IDs to key values.
    /// It extends the AbstractGenerator class and provides functionality to create this mapping via a Unity editor menu item.
    /// The generated JSON file is saved to a specified path within the project.
    /// </summary>
    public class AddressableKeyValueGenerator : AbstractGenerator
    {
        #region Fields

        private const string AssetPath = "Assets/Game/Data/Addressables";

        #endregion
        
        
        
        #region Menu Item

        [MenuItem("ActFit/Addressables/[Generate] Key Value Data with JSON (Initialize)", priority = 1)]
        public static void GenerateEnumKeyValueData()
        {
            var generator = new AddressableKeyValueGenerator();
            generator.Generate();
        }

        #endregion
        
        
        
        #region Protected Methods

        protected override void GenerateProcess()
        {
            var addressableEntries = GetAddressableAssetEntries();
            var addressableDataMap = GetAddressableDataMap(addressableEntries);

            if (addressableDataMap == null)
            {
                Debug.LogError("Addressable Data Map is null. Check AddressableGroup.");
                return;
            }

            EnsureDirectoryExists(AssetPath);

            var savePath = Path.Combine(AssetPath, "AddressableKeyValueData.json");
            var json = JsonConvert.SerializeObject(addressableDataMap, Formatting.Indented);
            File.WriteAllText(savePath, json);

            Debug.Log($"Addressables JSON saved to {savePath}");
        }

        #endregion
    }
}