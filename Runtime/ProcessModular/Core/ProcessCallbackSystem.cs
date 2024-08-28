
using System;
using System.Collections.Generic;
using UnityEngine;
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
            var cacheAssetLocation = cacheProvider.GetAssetKeysMap;
    
            _addressableSystem.LabelLocationMap[labelKey] = resourceLocations;

            // 주요 리소스(GameObject) 우선 처리
            foreach (var location in resourceLocations)
            {
                if (location.ResourceType != typeof(GameObject))
                {
                    continue;
                }
                
                if (cacheAssetLocation.TryGetValue(location.InternalId, out var addressableKey))
                {
                    _addressableSystem.KeyLocationMap[addressableKey] = location;
                }
            }

            // 나머지 리소스 처리
            foreach (var location in resourceLocations)
            {
                if (location.ResourceType == typeof(GameObject))
                {
                    continue;
                }
                
                if (cacheAssetLocation.TryGetValue(location.InternalId, out var addressableKey))
                {
                    _addressableSystem.KeyLocationMap.TryAdd(addressableKey, location);
                }
            }

            onSucceeded?.Invoke();
        }

        #endregion
    }
}