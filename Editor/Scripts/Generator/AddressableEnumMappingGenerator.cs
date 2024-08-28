
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// Generates a C# enum file based on previously generated addressable key-value JSON data.
    /// This generator reads the JSON data and converts it into an enum definition, saving it to a specified path.
    /// </summary>
    public sealed class AddressableEnumMappingGenerator : AbstractGenerator
    {
        protected override string AssetPath { get; set; } = RootAssetPath + "Scripts/Addressables/";

        /// <summary>
        /// Generates the enum mapping data from the existing key-value JSON data.
        /// This method is exposed as a Unity editor menu item.
        /// </summary>
        [MenuItem("ActFit/Addressables/[Generate] Addressable Enum Key (2-Step)", priority = 2)]
        public static void GenerateEnumMappingData()
        {
            new AddressableEnumMappingGenerator().Generate(true);
        }
        
        /// <summary>
        /// Executes the process of reading the JSON data, generating the enum file, 
        /// and saving it to the specified path.
        /// </summary>
        protected override void GenerateProcess()
        {
            if (!File.Exists(JsonAssetPath))
            {
                Debug.LogError("KeyValueData file not found. Ensure you have generated AddressableKeyValueJson.");
                return;
            }

            var jsonData = File.ReadAllText(JsonAssetPath);
            var entryDataMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            var keyFilePath = Path.Combine(AssetPath, "AddressableKey.cs");
            var directoryPath = Path.GetDirectoryName(keyFilePath);

            EditorExtensions.EnsureDirectoryExists(directoryPath);

            using (var fileWriter = new StreamWriter(keyFilePath))
            {
                fileWriter.WriteLine("public enum AddressableKey");
                fileWriter.WriteLine("{");

                foreach (var enumKey in entryDataMap.Values)
                {
                    string enumName = enumKey;
                    fileWriter.WriteLine($"    {enumName},");
                }

                fileWriter.WriteLine("}");
            }
            
            Debug.Log($"Enum generated successfully and saved to '{keyFilePath}'");
        }
    }
}