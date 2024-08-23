
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Handles callback processes for the Addressable Management System.
    ///
    /// Key Responsibilities:
    /// - Handle the completion of the Addressables initialization process.
    /// - Trigger specific actions upon successful or failed initialization.
    /// </summary>
    internal class ProcessCallback
    {
        // #region Fields
        //
        // private AddressableManagementSystem _coreManagementSystem;
        //
        // #endregion
        //
        //
        //
        // #region Constructor
        //
        // internal ProcessCallback(AddressableManagementSystem addressableManagementSystem)
        // {
        //     _coreManagementSystem = addressableManagementSystem;
        // }
        //
        // #endregion
        //
        //
        //
        // #region Callback Initialize Addressable
        //
        // internal void CBInitializeCompleted(IResourceLocator initializeAsyncResult
        //     , Action onSucceeded = null
        //     , Action onFailed = null)
        // {
        //     if (initializeAsyncResult == null)
        //     {
        //         onFailed?.Invoke();
        //         return;
        //     }
        //     
        //     onSucceeded?.Invoke();
        // }
        //
        // #endregion
        //
        //
        //
        // #region Callback Locations
        //
        // internal void CBLoadLocationCompleted(IList<IResourceLocation> resourceLocations
        //     , AssetLabelReference labelKey
        //     , ProcessForUniTask processForUniTask
        //     , Action onSucceeded = null
        //     , Action onFailed = null)
        // {
        //     if (resourceLocations == null || resourceLocations.Count == 0)
        //     {
        //         DeLog.LogWarning($"No resource locations found for label: {labelKey.labelString}");
        //         onFailed?.Invoke();
        //         return;
        //     }
        //     
        //     if (!_coreManagementSystem.AssetConfigDataMap.TryGetValue(labelKey, out var addressableDataValues))
        //     {
        //         // Setup have a AssetConfigDataMap
        //         addressableDataValues = new Dictionary<AddressableEnumKey, IResourceLocation>();
        //         _coreManagementSystem.AssetConfigDataMap[labelKey] = addressableDataValues;
        //         
        //         // Setup Temp Data
        //         _coreManagementSystem.AssetTempMap[labelKey] = resourceLocations;
        //     }
        //
        //     var isSucceeded = false;
        //     foreach (var resourceLocation in resourceLocations)
        //     {
        //         if (processForUniTask.MappingConfig.TryGetValue(resourceLocation.InternalId, out var addressableKey))
        //         {
        //             if (!addressableDataValues.TryAdd(addressableKey, resourceLocation))
        //             {
        //                 continue;
        //             }
        //
        //             isSucceeded = true;
        //         }
        //         else
        //         {
        //             DeLog.LogError($"No matching addressable enum Key for location : {resourceLocation.InternalId}");
        //         }
        //     }
        //
        //     if (isSucceeded)
        //     {
        //         onSucceeded?.Invoke();
        //     }
        //     else
        //     {
        //         onFailed?.Invoke();
        //     }
        // }
        //
        // #endregion
    }
}
