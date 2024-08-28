
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class ModifyPostProcessor
    {
        private const string AssetPath = "Assets/AddressableSystemSetting.asset";
        
        [InitializeOnLoadMethod]
        static void OnInitializeDelayCall()
        {
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
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            EditorApplication.delayCall += AddressableCacheGenerator.GenerateAddressableCacheSO;
        }

        private static bool CanWriteAccess(AddressableAssetSettings.ModificationEvent evt)
        {
            return evt is AddressableAssetSettings.ModificationEvent.EntryModified 
                or AddressableAssetSettings.ModificationEvent.LabelAdded 
                or AddressableAssetSettings.ModificationEvent.LabelRemoved;
        }
    }
}