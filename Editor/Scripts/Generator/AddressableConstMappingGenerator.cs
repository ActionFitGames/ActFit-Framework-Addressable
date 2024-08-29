
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    /// <summary>
    /// Generates a C# class file with constant integer values based on previously generated addressable key-value JSON data.
    /// This generator reads the JSON data and converts it into a class definition, saving it to a specified path.
    /// </summary>
    public sealed class AddressableConstMappingGenerator : AbstractGenerator
    {
        protected override string AssetPath { get; set; } = RootAssetPath + "Scripts/Addressables/";

        /// <summary>
        /// Generates the constant mapping data from the existing key-value JSON data.
        /// This method is exposed as a Unity editor menu item.
        /// </summary>
        [MenuItem("ActFit/Addressables/[Generate] Addressable Key Constants (2-Step)", priority = 2)]
        public static void GenerateConstMappingData()
        {
            new AddressableConstMappingGenerator().Generate();
        }
        
        /// <summary>
        /// Executes the process of reading the JSON data, generating the class file with constants,
        /// and saving it to the specified path.
        /// </summary>
        protected override void GenerateProcess()
        {
            if (!File.Exists(JsonIntegerAssetPath))
            {
                Debug.LogError("Integer Key-Value Data file not found. Ensure you have generated AddressableKeyValueJson with integer mappings.");
                return;
            }

            var jsonData = File.ReadAllText(JsonIntegerAssetPath);
            var entryDataMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);
            var keyFilePath = Path.Combine(AssetPath, "AddrKey.cs");
            var directoryPath = Path.GetDirectoryName(keyFilePath);

            EditorExtensions.EnsureDirectoryExists(directoryPath);

            using (var fileWriter = new StreamWriter(keyFilePath))
            {
                fileWriter.WriteLine("public static class AddrKey");
                fileWriter.WriteLine("{");

                foreach (var kvp in entryDataMap)
                {
                    string constantName = kvp.Key;
                    int constantValue = kvp.Value;
                    fileWriter.WriteLine($"    public const int {constantName} = {constantValue};");
                }

                fileWriter.WriteLine("}");
            }
            
            Debug.Log($"Constants generated successfully and saved to '{keyFilePath}'");
        }
    }
}