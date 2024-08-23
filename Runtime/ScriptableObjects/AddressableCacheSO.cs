
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor.AddressableAssets;
#endif

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// ScriptableObject used to cache addressable asset locations for faster access.
    /// This class maintains mappings between asset labels and their associated resource locations,
    /// as well as mappings between specific addressable keys and their resource locations.
    /// </summary>
    // [CreateAssetMenu(fileName = "AddressableCache", menuName = "ActFit/Addressables/CacheSO")]
    public class AddressableCacheSO : ScriptableObject
    {
        #region Fields
        
        [SerializeField] [TextArea] private string _description = "[어드레서블 데이터 캐시 컨피그 파일]\n" +
                                                                  "런타임 리소스 비용을 최소화 하기 위함\n" +
                                                                  "최신 데이터가 아닐 경우 로그 발행\n";

        [SerializedDictionary("AssetLabelReference", "[NoEdit] Internal ID (List)")]
        public SerializedDictionary<AssetLabelReference, List<string>> LabelLocationsMap;
        
        [SerializedDictionary("Addressable Key", "[NoEdit] IResourceLocation")]
        public SerializedDictionary<AddressableKey, string> AssetLocationMap;

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
