
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    public static class ModifyPostProcessor
    {
        private const string AssetPath = "Assets/AddressableSystemSetting.asset";
        private const string EditorReloadFlagKey = "AddressableSystem_EditorReloadFlag";
        
        [InitializeOnLoadMethod]
        static void OnInitializeDelayCall()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
            {
                return;
            }
            
            var setting = AssetDatabase.LoadAssetAtPath<AddressableSettingSO>(AssetPath);
            
            if (!setting)
            {
                Debug.LogWarning("[Addressable Modify-Post-Processor] Setting not found. Forcing recompile to trigger initialization.");
                ForceReimportAsset(AssetPath);
                return;
            }

            if (!setting.IsAutoUpdate)
            {
                Debug.LogWarning("[Addressable Modify-Post-Processor] Auto-update disabled, you can manually updated.");
                return;
            }
            
            Debug.Log("[Addressable Modify-Post-Processor] Constructor called, PostProcess enabled.");
            
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (addressableSettings)
            {
                addressableSettings.OnModification += OnAddressableSettingModified;
            }
        }
        
        private static void ForceReimportAsset(string assetPath)
        {
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            
            Debug.Log("[Addressable Modify-Post-Processor] Forced recompile triggered.");
        }

        private static void OnAddressableSettingModified(
            AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent evt, object obj)
        {
            if (!CanWriteAccess(evt))
            {
                return;
            }

            if (evt is AddressableAssetSettings.ModificationEvent.LabelAdded
                or AddressableAssetSettings.ModificationEvent.LabelRemoved)
            {
                AddressableCacheGenerator.GenerateAddressableCacheSO();
                AssetDatabase.Refresh();
                return;
            }
            
            AddressableJsonMappingGenerator.GenerateJsonKeyValueData();
            AddressableEnumMappingGenerator.GenerateEnumMappingData();
            EditorPrefs.SetBool(EditorReloadFlagKey, true);
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
            {
                return;
            }
            
            if (!EditorPrefs.GetBool(EditorReloadFlagKey, false))
            {
                return;
            }
            
            EditorApplication.delayCall += () =>
            {
                AddressableCacheGenerator.GenerateAddressableCacheSO();
                EditorPrefs.SetBool(EditorReloadFlagKey, false);
            };
        }

        private static bool CanWriteAccess(AddressableAssetSettings.ModificationEvent evt)
        {
            return evt is AddressableAssetSettings.ModificationEvent.EntryModified 
                or AddressableAssetSettings.ModificationEvent.LabelAdded 
                or AddressableAssetSettings.ModificationEvent.LabelRemoved;
        }
    }
}