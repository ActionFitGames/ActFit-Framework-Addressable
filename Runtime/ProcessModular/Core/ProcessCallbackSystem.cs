
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal class ProcessCallbackSystem
    {
        private readonly AddressableSystem _addressableSystem;
        
        internal ProcessCallbackSystem(AddressableSystem addressableSystem)
        {
            _addressableSystem = addressableSystem;
        }

        #region Callbacks

        /// <summary>
        /// Handles the callback for when the Addressables system initialization is complete.
        /// Executes the appropriate success or failure action based on the initialization result.
        /// </summary>
        /// <param name="initializeAsyncResult">The result of the initialization process.</param>
        /// <param name="onSucceeded">Action to execute on successful initialization.</param>
        /// <param name="onFailed">Action to execute on failed initialization.</param>
        internal void CallbackInitialized(IResourceLocator initializeAsyncResult
            , Action onSucceeded = null
            , Action onFailed = null)
        {
            if (initializeAsyncResult == null)
            {
                onFailed?.Invoke();
                return;
            }
            
            onSucceeded?.Invoke();
        }

        /// <summary>
        /// Handles the callback for when resource locations are successfully loaded.
        /// Maps the loaded resource locations to the corresponding labels and keys in the system.
        /// </summary>
        /// <param name="resourceLocations">List of resource locations loaded.</param>
        /// <param name="labelKey">The label associated with the resource locations.</param>
        /// <param name="onSucceeded">Action to execute on successful load of locations.</param>
        /// <param name="onFailed">Action to execute on failed load of locations.</param>
        internal void CallbackLocationsLoaded(IList<IResourceLocation> resourceLocations
            , string labelKey
            , Action onSucceeded = null
            , Action onFailed = null)
        {
            if (resourceLocations == null || resourceLocations.Count == 0)
            {
                DeLog.LogWarning($"No resource locations found for label: {labelKey}");
                onFailed?.Invoke();
                return;
            }
    
            var cacheProvider = AddressableMonoBehavior.Cache;
            var cacheAssetKeysMap = cacheProvider.GetAssetKeysMap;

            if (!_addressableSystem.LabelAssetKeyLocationMap.ContainsKey(labelKey))
            {
                _addressableSystem.LabelAssetKeyLocationMap[labelKey] = new List<ResourceKvp>();
            }
            
            foreach (var resourceLocation in resourceLocations)
            {
                var primaryKey = $"{resourceLocation.ResourceType}{resourceLocation.PrimaryKey}";

                if (!cacheAssetKeysMap.TryGetValue(primaryKey, out var addressableKey))
                {
                    continue;
                }
                
                var resourceKvp = new ResourceKvp
                {
                    AddressableKey = addressableKey,
                    ResourceLocation = resourceLocation
                };

                _addressableSystem.LabelAssetKeyLocationMap[labelKey].Add(resourceKvp);
                _addressableSystem.AssetKeyLocationMap.TryAdd(addressableKey, resourceLocation);
            }

            onSucceeded?.Invoke();
        }

        #endregion
    }
}