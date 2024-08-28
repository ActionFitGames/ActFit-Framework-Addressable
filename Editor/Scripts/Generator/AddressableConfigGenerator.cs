
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// The AddressableConfigGenerator class is responsible for generating a ScriptableObject that caches Addressable resource locations.
    /// It extends the AbstractGenerator class and provides functionality to create this cache configuration via a Unity editor menu item.
    /// </summary>
    public class AddressableConfigGenerator : AbstractGenerator
    {
        #region Fields

        private const string AssetPath = "Assets/Resources/ScriptableObjects/Addressables";

        #endregion

        
        
        #region Menu Item

        /// <summary>
        /// Generates the Addressable Cache Config ScriptableObject via a Unity editor menu item.
        /// This method is the entry point for creating the cache configuration.
        /// </summary>
        [MenuItem("ActFit/Addressables/[Generate] Addressable Cache Config (SO)", priority = 12)]
        public static void GenerateLocationCachedData()
        {
            var generator = new AddressableConfigGenerator();
            generator.Generate();
        }

        #endregion

        
        
        #region Protected Methods

        /// <summary>
        /// The main process for generating the Addressable cache configuration.
        /// It ensures the necessary directory exists, retrieves Addressable labels, 
        /// and creates mappings for label locations and asset locations, which are stored in a ScriptableObject.
        /// </summary>
        protected override void GenerateProcess()
        {
            EnsureDirectoryExists(AssetPath);

            var cachedSO = CreateAddressableCacheSO();
            var allLabels = GetAddressableLabelReferences();
            var keyPair = AddressablePairFactory.GetAddressableKeysMap();
            var labelLocationMap = new SerializedDictionary<string, List<string>>();
            var assetLocationMap = new SerializedDictionary<string, AddressableKey>();
            cachedSO.LabelReferencesString.Clear();
            
            foreach (var labelReference in allLabels)
            {
                cachedSO.LabelReferencesString.Add(labelReference.labelString);
                var resourceLocations = GetResourceLocations(labelReference);
                if (resourceLocations is not { Count: > 0 })
                {
                    continue;
                }
                
                var internalIDs = new List<string>();
                foreach (var location in resourceLocations)
                {
                    if (!keyPair.TryGetValue(location.InternalId, out var addressableKey))
                    {
                        continue;
                    }

                    internalIDs.Add(location.InternalId); 
                    assetLocationMap.TryAdd(location.InternalId, addressableKey);
                }

                labelLocationMap[labelReference.labelString] = internalIDs;
            }

            cachedSO.LabelLocationsMap = labelLocationMap;
            cachedSO.AssetLocationMap = assetLocationMap;
            
            EditorUtility.SetDirty(cachedSO);
            AssetDatabase.SaveAssets();

            Debug.Log("[Cache] Addressable Cache Config is generated. (Complete!)");
        }
        
        /// <summary>
        /// Creates or loads the AddressableCache ScriptableObject used to store cached data.
        /// If the ScriptableObject already exists, it will be updated.
        /// </summary>
        /// <returns>Returns the AddressableCacheSO object.</returns>
        private AddressableCacheSO CreateAddressableCacheSO()
        {
            var path = $"{AssetPath}/AddressableCache.asset";
            var cacheSO = AssetDatabase.LoadAssetAtPath<AddressableCacheSO>(path);

            if (!cacheSO)
            {
                cacheSO = ScriptableObject.CreateInstance<AddressableCacheSO>();
                AssetDatabase.CreateAsset(cacheSO, path);
                Debug.Log($"Created AddressableCacheSO at {path}");
            }
            else
            {
                Debug.Log("AddressableCacheSO already exists and will be [Update]");
            }

            return cacheSO;
        }

        /// <summary>
        /// Retrieves all Addressable labels defined in the Addressable settings.
        /// Converts each label into an AssetLabelReference for use in caching.
        /// </summary>
        /// <returns>Returns a list of AssetLabelReference objects.</returns>
        public static List<AssetLabelReference> GetAddressableLabelReferences()
        {
            var labels = Settings.GetLabels(); // Assuming Settings.GetLabels() returns a List<string>
            return labels.Select(label => new AssetLabelReference() { labelString = label }).ToList();
        }

        /// <summary>
        /// Retrieves the resource locations associated with a given Addressable label.
        /// This method loads the resource locations synchronously and returns the result.
        /// </summary>
        /// <param name="assetLabelReference">The label for which resource locations are retrieved.</param>
        /// <returns>Returns a list of IResourceLocation objects associated with the label.</returns>
        private IList<IResourceLocation> GetResourceLocations(AssetLabelReference assetLabelReference)
        {
            var locationHandle = Addressables.LoadResourceLocationsAsync(assetLabelReference);
            locationHandle.WaitForCompletion();

            return locationHandle.Result;
        }

        #endregion
    }
}