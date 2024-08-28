
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    [InitializeOnLoad]
    public sealed class SettingInitializer
    {
        private const string AssetRootPath = "Assets/";
        private const string FileName = "AddressableSystemSetting.asset";
        private const string ResourcePath = "Assets/Resources/ScriptableObjects/Addressables/";
        private const string ConvertFileName = "AddressableSystemSettingConvert.asset";
        
        static SettingInitializer()
        {
            var settingFullPath = AssetRootPath + FileName;
            var setting = AssetDatabase.LoadAssetAtPath<AddressableSettingSO>(settingFullPath);

            if (!setting)
            {
                setting = ScriptableObject.CreateInstance<AddressableSettingSO>();

                AssetDatabase.CreateAsset(setting, settingFullPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[Addressable System] Setting created. Initialize Completed. {settingFullPath}");
            }

            var convertFullPath = ResourcePath + ConvertFileName;
            var convertSetting = AssetDatabase.LoadAssetAtPath<AddressableSettingConvertSO>(convertFullPath);

            if (!convertSetting)
            {
                EditorExtensions.EnsureDirectoryExists(convertFullPath);

                convertSetting = ScriptableObject.CreateInstance<AddressableSettingConvertSO>();

                AssetDatabase.CreateAsset(convertSetting, convertFullPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[Addressable System] Convert Setting created. Initialize Completed. {convertFullPath}");
            }

            AssetDatabase.Refresh();
        }
    }
}