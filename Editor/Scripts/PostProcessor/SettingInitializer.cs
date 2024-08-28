
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    [InitializeOnLoad]
    public class SettingInitializer
    {
        private const string AssetRootPath = "Assets/";
        private const string FileName = "AddressableSystemSetting.asset";
        private const string ResourcePath = "Assets/Resources/ScriptableObjects/Addressables/";
        private const string ConvertFileName = "AddressableSystemSettingConvert.asset";
        
        static SettingInitializer()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
            {
                return;
            }
            
            var settingFullPath = AssetRootPath + FileName;
            var convertFullPath = ResourcePath + ConvertFileName;

            // 두 SO 파일이 모두 존재하는지 확인
            var setting = AssetDatabase.LoadAssetAtPath<AddressableSettingSO>(settingFullPath);
            var convertSetting = AssetDatabase.LoadAssetAtPath<AddressableSettingConvertSO>(convertFullPath);

            if (setting && convertSetting)
            {
                Debug.Log("[Addressable System] Both settings already exist. No initialization needed.");
                return;
            }

            // AddressableSystemSetting.asset 생성
            if (!setting)
            {
                setting = ScriptableObject.CreateInstance<AddressableSettingSO>();
                AssetDatabase.CreateAsset(setting, settingFullPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Addressable System] Setting created. Initialize Completed. {settingFullPath}");
            }

            // AddressableSystemSettingConvert.asset 생성
            if (!convertSetting)
            {
                EditorExtensions.EnsureDirectoryExists(ResourcePath);
                convertSetting = ScriptableObject.CreateInstance<AddressableSettingConvertSO>();
                AssetDatabase.CreateAsset(convertSetting, convertFullPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Addressable System] Convert Setting created. Initialize Completed. {convertFullPath}");
            }

            // 에셋 데이터베이스 새로고침
            AssetDatabase.Refresh();
        }
    }
}