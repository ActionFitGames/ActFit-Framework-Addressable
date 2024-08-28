using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    [InitializeOnLoad]
    public static class AddressablePostInitializer
    {
        private const string AssetPath = "Assets/";
        private const string FileName = "AddressableSystemSetting.asset";
        private const string ResourcePath = "Assets/Resources/ScriptableObjects/Addressables/";
        private const string ConvertFileName = "AddressableSystemSettingConvert.asset";

        #region Constructor

        static AddressablePostInitializer()
        {
            CreateAddressableGlobalSettingIfNotExist();
            CreateAddressableConvertSettingIfNotExist();
        }

        public static void ForceInstantiate()
        {
            CreateAddressableGlobalSettingIfNotExist();
            CreateAddressableConvertSettingIfNotExist();
        }

        #endregion

        private static void CreateAddressableGlobalSettingIfNotExist()
        {
            var fullPath = AssetPath + FileName;
            var setting = AssetDatabase.LoadAssetAtPath<AddressableSettingSO>(fullPath);

            if (!setting)
            {
                setting = ScriptableObject.CreateInstance<AddressableSettingSO>();

                AssetDatabase.CreateAsset(setting, fullPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[Addressable System] Setting created. Initialize Completed. {fullPath}");
            }
        }

        private static void CreateAddressableConvertSettingIfNotExist()
        {
            var fullPath = ResourcePath + ConvertFileName;
            var convertSetting = AssetDatabase.LoadAssetAtPath<AddressableSettingConvertSO>(fullPath);

            if (!convertSetting)
            {
                convertSetting = ScriptableObject.CreateInstance<AddressableSettingConvertSO>();

                EnsureDirectoryExists(ResourcePath);
                AssetDatabase.CreateAsset(convertSetting, fullPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[Addressable System] Convert Setting created. Initialize Completed. {fullPath}");
            }
        }

        private static void EnsureDirectoryExists(string path)
        {
            string[] folders = path.Split('/');
            string currentPath = "";

            for (int i = 0; i < folders.Length; i++)
            {
                if (string.IsNullOrEmpty(folders[i]))
                    continue;

                currentPath = string.IsNullOrEmpty(currentPath) ? folders[i] : $"{currentPath}/{folders[i]}";

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(currentPath), System.IO.Path.GetFileName(currentPath));
                }
            }

            AssetDatabase.Refresh();
                
            Debug.Log($"Ensured directory exists: {path}");
        }
    }
}