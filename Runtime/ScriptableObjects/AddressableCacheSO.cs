
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
#endif

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// ScriptableObject used to cache addressable asset locations for faster access.
    /// This class maintains mappings between asset labels and their associated resource locations,
    /// as well as mappings between specific addressable keys and their resource locations.
    /// </summary>
    // [CreateAssetMenu(fileName = "AddressableCache", menuName = "ActFit/Addressables/CacheSO")]
    public class AddressableCacheSO : ScriptableObject, ICacheProvider
    {
        #region Fields
        
#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
        [SerializeField] [TextArea] private string _description = "Addressable IResourceLocation caching internalID\n" +
#pragma warning restore CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
                                                                  "  -- This is generate Enum 'AddressableKey'\n" +
                                                                  "  -- Runtime, Enum Key Mapping to IResourceLocation\n";
        
        public List<string> LabelStrings;
        
        [SerializedDictionary("[NoEdit] Unique Composite Key", "[Enum] Addressable Key")]
        public SerializedDictionary<string, AddressableKey> AssetKeysMap;
        
        /// <summary> Implements 'ICacheProvider' </summary>
        public List<string> GetLabelStrings => LabelStrings;
        public Dictionary<string, AddressableKey> GetAssetKeysMap => AssetKeysMap;

        #endregion



#if UNITY_EDITOR
        // private void OnValidate()
        // {
        //     if (LabelLocationsMap == null || LabelLocationsMap.Count == 0)
        //     {
        //         Debug.LogWarning("LabelLocationsMap is empty. Please regenerate the cache.");
        //         return;
        //     }
        //
        //     var actualLabels = GetActualLabels();
        //
        //     foreach (var actualLabel in actualLabels)
        //     {
        //         var cachedEntry = LabelLocationsMap.FirstOrDefault(entry => entry.Key.labelString == actualLabel);
        //
        //         if (cachedEntry.Key == null)
        //         {
        //             Debug.LogError($"The label '{actualLabel}' exists in the Addressable system but is missing from the cache.\n" +
        //                            " Please update the AddressableCacheSO.");
        //             return;
        //         }
        //
        //         var actualLocations = GetActualResourceLocations(actualLabel);
        //
        //         if (cachedEntry.Value.Count != actualLocations.Count)
        //         {
        //             Debug.LogError($"Mismatch in resource locations for label '{actualLabel}'.\n" +
        //                            $"Cached: {cachedEntry.Value?.Count ?? 0}, Actual: {actualLocations.Count}\n." +
        //                            "Please update the AddressableCacheSO.");
        //             return;
        //         }
        //     }
        // }
        //
        // private List<string> GetActualLabels()
        // {
        //     var settings = AddressableAssetSettingsDefaultObject.Settings;
        //     return settings.GetLabels().ToList();
        // }
        //
        // private IList<IResourceLocation> GetActualResourceLocations(string label)
        // {
        //     var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(object));
        //     locationHandle.WaitForCompletion();
        //
        //     return locationHandle.Result;
        // }
#endif
    }
}
