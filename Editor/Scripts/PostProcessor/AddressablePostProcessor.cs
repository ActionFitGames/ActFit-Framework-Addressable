
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Compilation;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// This class handles post-processing tasks for the Addressable system within the ActFitFramework.
    /// It subscribes to modification events in Addressable settings and ensures that necessary operations
    /// are completed after script recompilation and editor reloads.
    /// </summary>
    [InitializeOnLoad]
    public class AddressablePostProcessor
    {
        private const string PendingConfigGenerationKey = "ActFitFramework.PendingConfigGeneration";

        #region Constructor Processor

        static AddressablePostProcessor()
        {
            if (!AddressableSettingSO.Instance.IsAutoUpdate)
            {
                Debug.LogWarning("[AddressablePostProcessor] Auto-update disabled, you can manually updated.");
                return;
            }
            
            Debug.Log("[AddressablePostProcessor] Constructor called, PostProcess enabled.");
            
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            if (addressableSettings)
            {
                addressableSettings.OnModification += OnAddressableSettingsModified;
            }

            // 스크립트가 로드될 때 혹시 남아있는 플래그가 있으면 처리
            // if (EditorPrefs.GetBool(PendingConfigGenerationKey, false))
            // {
            //     EditorApplication.delayCall += () =>
            //     {
            //         AddressableConfigGenerator.GenerateLocationCachedData();
            //         EditorPrefs.SetBool(PendingConfigGenerationKey, false);
            //     };
            // }
        }

        #endregion

        
        
        #region Addressable Modified

        /// <summary>
        /// This method is triggered when Addressable settings are modified.
        /// It generates key-value data and schedules tasks that need to be executed after script compilation.
        /// </summary>
        /// <param name="settings">The modified Addressable asset settings.</param>
        /// <param name="evt">The type of modification event.</param>
        /// <param name="obj">The object involved in the modification.</param>
        private static void OnAddressableSettingsModified(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent evt, object obj)
        {
            if (evt != AddressableAssetSettings.ModificationEvent.EntryModified
                && evt != AddressableAssetSettings.ModificationEvent.LabelAdded
                && evt != AddressableAssetSettings.ModificationEvent.LabelRemoved)
            {
                return;
            }

            if (evt is AddressableAssetSettings.ModificationEvent.LabelAdded or AddressableAssetSettings.ModificationEvent.LabelRemoved)
            {
                AddressableConfigGenerator.GenerateLocationCachedData();
                AssetDatabase.Refresh();
                return;
            }
            
            AddressableKeyValueGenerator.GenerateEnumKeyValueData();
            AddressableEnumKeyGenerator.GenerateAddressableEnumKey();

            // 컴파일 후 작업 예약
            EditorPrefs.SetBool(PendingConfigGenerationKey, true);
            CompilationPipeline.compilationFinished += OnCompiledFinished;
        }

        private static void OnCompiledFinished(object obj)
        {
            // 스크립트가 다시 로드된 후 실행하기 위해 flag 사용
            if (EditorPrefs.GetBool(PendingConfigGenerationKey, false))
            {
                EditorApplication.delayCall += () =>
                {
                    AddressableConfigGenerator.GenerateLocationCachedData();
                    EditorPrefs.SetBool(PendingConfigGenerationKey, false);
                };
            }

            CompilationPipeline.compilationFinished -= OnCompiledFinished;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // 스크립트가 다시 로드된 후에도 작업이 필요하면 실행
            if (EditorPrefs.GetBool(PendingConfigGenerationKey, false))
            {
                EditorApplication.delayCall += () =>
                {
                    AddressableConfigGenerator.GenerateLocationCachedData();
                    EditorPrefs.SetBool(PendingConfigGenerationKey, false);
                };
            }
        }

        #endregion
    }
}