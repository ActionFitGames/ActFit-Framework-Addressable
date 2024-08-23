
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// The AddressableEnumKeyGenerator class is responsible for generating a C# enum based on key-value data from a JSON file.
    /// It extends the AbstractGenerator class and provides functionality to create an enum file via a Unity editor menu item.
    /// The generated enum is saved to a specified path within the project, allowing for easy referencing of Addressable keys as strongly-typed enums.
    /// </summary>
    public class AddressableEnumKeyGenerator : AbstractGenerator
    {
        #region Fields

        private const string AssetPath = "Assets/Game/Scripts/Addressables";
        private const string JsonAssetPath = "Assets/Game/Data/Addressables/AddressableKeyValueData.json";

        #endregion
        
        
        
        #region Menu Item

        [MenuItem("ActFit/Addressables/[Generate] Addressable Enum Key", priority = 11)]
        public static void GenerateAddressableEnumKey()
        {
            var generator = new AddressableEnumKeyGenerator();
            generator.Generate(true);
        }

        #endregion
        
        
        
        #region Protected Methods

        protected override void GenerateProcess()
        {
            if (!File.Exists(JsonAssetPath))
            {
                Debug.LogError("KeyValueData file not found. Ensure you have generated AddressableKeyValue.");
                return;
            }

            var jsonData = File.ReadAllText(JsonAssetPath);
            var addressableDataMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            var keyFilePath = Path.Combine(AssetPath, "AddressableKey.cs");
            var directoryPath = Path.GetDirectoryName(keyFilePath);

            EnsureDirectoryExists(directoryPath);

            using (var fileWriter = new StreamWriter(keyFilePath))
            {
                fileWriter.WriteLine("public enum AddressableKey");
                fileWriter.WriteLine("{");

                foreach (var key in addressableDataMap.Values)
                {
                    string enumName = ConvertFileName(key);
                    fileWriter.WriteLine($"    {enumName},");
                }

                fileWriter.WriteLine("}");
            }

            Debug.Log($"Enum generated successfully and saved to '{keyFilePath}'");
        }

        #endregion
    }
}