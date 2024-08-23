
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Handles asynchronous operations related to Addressables using UniTask.
    ///
    /// Key Responsibilities:
    /// - Load assets asynchronously using Addressables.
    /// - Manage asset references and ensure proper memory management.
    /// - Provide utilities for checking the status of asset loading operations.
    /// - Unload assets when they are no longer needed to free up memory.
    /// </summary>
    internal class ProcessForUniTask
    {
        // #region Fields
        //
        // private readonly AddressableManagementSystem _coreManagementSystem;
        // private readonly ProcessCallback _processCallback;
        // private readonly ExceptionHandleTypes _exceptionHandleType;
        //
        // #endregion
        //
        //
        //
        // #region Constructor & Initialize
        //
        // internal ProcessForUniTask(AddressableManagementSystem addressableManagementSystem)
        // {
        //     _coreManagementSystem = addressableManagementSystem;
        //     _processCallback = new ProcessCallback(_coreManagementSystem);
        //     _exceptionHandleType = _coreManagementSystem.ExceptionHandleType;
        // }
        //
        // internal async UniTask<IResourceLocator> InitializeInternal(bool autoReleaseHandle = true, Action<float> onProgress = null)
        // {
        //     try
        //     {
        //         var initializeHandle = Addressables.InitializeAsync(false);
        //         var initializeTask = initializeHandle.ToUniTask();
        //
        //         while (!initializeHandle.IsDone)
        //         {
        //             onProgress?.Invoke(initializeHandle.PercentComplete);
        //             await UniTask.Yield();
        //         }
        //
        //         var initializeAsyncResult = await initializeTask;
        //
        //         _processCallback.CBInitializeCompleted(initializeAsyncResult);
        //
        //         if (!autoReleaseHandle)
        //         {
        //             return initializeAsyncResult;
        //         }
        //
        //         Addressables.Release(initializeHandle);
        //         return initializeAsyncResult;
        //     }
        //     catch (Exception exception)
        //     {
        //         DeLog.LogError("An error occurred during initialization.");
        //         DeLogHandler.DeLogException(exception, _exceptionHandleType);
        //         return null;
        //     }
        // }
        //
        // #endregion
        //
        //
        //
        // #region Load IResourceLocation (Obsolete)
        //
        // [Obsolete("Deprecated since AddressableCacheSO was introduced.")]
        // internal async UniTask<IList<IResourceLocation>> LoadLocationsAsync(AssetLabelReference labelReference, Type type = null)
        // {
        //     if (labelReference == null)
        //     {
        //         DeLogHandler.DeLogInvalidLabelException(_exceptionHandleType);
        //         return null;
        //     }
        //
        //     try
        //     {
        //         var loadLocationHandle = Addressables.LoadResourceLocationsAsync(labelReference, type).ToUniTask();
        //         var loadLocationResult = await loadLocationHandle;
        //
        //         _processCallback.CBLoadLocationCompleted(loadLocationResult, labelReference, this);
        //         return loadLocationResult;
        //     }
        //     catch (Exception exception)
        //     {
        //         DeLogHandler.DeLogException(exception, _exceptionHandleType);
        //         return null;
        //     }
        // }
        //
        // #endregion
        //
        //
        //
        // #region Load Asset - Multiple
        //
        // internal async UniTask LoadAssetsAsync<T>(AssetLabelReference labelReference
        //     , Action<float> onProgress = null
        //     , Action<T> onCallbackLoaded = null
        //     , Action onCallbackCompleted = null) where T : Object
        // {
        //     if (labelReference == null)
        //     {
        //         DeLogHandler.DeLogInvalidLabelException(_exceptionHandleType);
        //         return;
        //     }
        //
        //     try
        //     {
        //         if (!_coreManagementSystem.AssetConfigDataMap.TryGetValue(labelReference, out var addressableDataValue))
        //         {
        //             DeLog.LogError("Addressable Data Value does not exist or is not used by this label.");
        //             return;
        //         }
        //
        //         if (!_coreManagementSystem.AssetTempMap.TryGetValue(labelReference, out var resourceLocations))
        //         {
        //             DeLog.LogError("No temporary resource locations found for the given label.");
        //             return;
        //         }
        //
        //         var loadAssetsHandle =
        //             Addressables.LoadAssetsAsync<T>(resourceLocations, asset => { onCallbackLoaded?.Invoke(asset); });
        //
        //         while (!loadAssetsHandle.IsDone)
        //         {
        //             onProgress?.Invoke(loadAssetsHandle.PercentComplete);
        //             await UniTask.Yield();
        //         }
        //
        //         await loadAssetsHandle;
        //
        //         Addressables.Release(loadAssetsHandle);
        //         onCallbackCompleted?.Invoke();
        //         _coreManagementSystem.AssetTempMap.Clear();
        //     }
        //     catch (Exception exception)
        //     {
        //         DeLogHandler.DeLogException(exception, _exceptionHandleType);
        //     }
        // }
        //
        // #endregion
        //
        // #region Load Asset - Single
        //
        // internal async UniTask LoadAssetAsync<T>(string key
        //     , Action<float> onProgress = null
        //     , Action<T> onCallbackLoaded = null
        //     , Action onCallbackCompleted = null) where T : Object
        // {
        //     if (string.IsNullOrEmpty(key))
        //     {
        //         DeLog.LogError("Invalid key provided.");
        //         return;
        //     }
        //
        //     try
        //     {
        //         var loadAssetHandle = Addressables.LoadAssetAsync<T>(key);
        //
        //         while (!loadAssetHandle.IsDone)
        //         {
        //             onProgress?.Invoke(loadAssetHandle.PercentComplete);
        //             await UniTask.Yield();
        //         }
        //
        //         var asset = await loadAssetHandle;
        //
        //         onCallbackLoaded?.Invoke(asset);
        //         onCallbackCompleted?.Invoke();
        //         Addressables.Release(loadAssetHandle);
        //     }
        //     catch (Exception exception)
        //     {
        //         DeLogHandler.DeLogException(exception, _exceptionHandleType);
        //     }
        // }
        //
        // #endregion
        //
        // #region Get Assets
        //
        // /// <summary>
        // /// Retrieves an asset of type T using the specified AddressableEnumKey and optional AssetLabelReference.
        // /// If the AssetLabelReference is not provided, the method will iterate over all entries in the dictionary,
        // /// which may result in a performance hit depending on the size of the dictionary.
        // /// Note: This method should not be used for GameObject types. Use the Instantiate method instead.
        // /// </summary>
        // internal T GetAsset<T>(AddressableEnumKey addressableEnumKey, AssetLabelReference labelReference = null) where T : Object
        // {
        //     if (typeof(T) == typeof(GameObject))
        //     {
        //         DeLog.LogError($"Cannot use GetAsset for GameObject type. Use Instantiate method instead for key: {addressableEnumKey}");
        //         return null;
        //     }
        //
        //     var resourceLocation = GetResourceLocation(addressableEnumKey, labelReference);
        //     return resourceLocation != null ? LoadAssetInternal<T>(resourceLocation, addressableEnumKey) : null;
        // }
        //
        // /// <summary>
        // /// Retrieves an asset of type T using the specified AddressableEnumKey and optional labelString.
        // /// If the labelString is not provided, the method will iterate over all entries in the dictionary,
        // /// which may result in a performance hit depending on the size of the dictionary.
        // /// Note: This method should not be used for GameObject types. Use the Instantiate method instead.
        // /// </summary>
        // internal T GetAsset<T>(AddressableEnumKey addressableEnumKey, string labelString = null) where T : Object
        // {
        //     if (typeof(T) == typeof(GameObject))
        //     {
        //         DeLog.LogError($"Cannot use GetAsset for GameObject type. Use Instantiate method instead for key: {addressableEnumKey}");
        //         return null;
        //     }
        //
        //     var resourceLocation = GetResourceLocation(addressableEnumKey, null, labelString);
        //     return resourceLocation != null ? LoadAssetInternal<T>(resourceLocation, addressableEnumKey) : null;
        // }
        //
        // #endregion
        //
        //
        //
        // #region Instantiate
        //
        // /// <summary>
        // /// Instantiates a GameObject using the specified AddressableEnumKey and optional AssetLabelReference.
        // /// If the AssetLabelReference is not provided, the method will iterate over all entries in the dictionary,
        // /// which may result in a performance hit depending on the size of the dictionary.
        // /// </summary>
        // internal GameObject Instantiate(AddressableEnumKey addressableEnumKey, AssetLabelReference labelReference = null
        //     , Transform parent = null, bool instantiateInWorldSpace = false)
        // {
        //     var resourceLocation = GetResourceLocation(addressableEnumKey, labelReference);
        //     return resourceLocation != null
        //         ? InstantiateInternal(resourceLocation, addressableEnumKey, parent, instantiateInWorldSpace)
        //         : null;
        // }
        //
        // /// <summary>
        // /// Instantiates a GameObject using the specified AddressableEnumKey and optional labelString.
        // /// If the labelString is not provided, the method will iterate over all entries in the dictionary,
        // /// which may result in a performance hit depending on the size of the dictionary.
        // /// </summary>
        // internal GameObject Instantiate(AddressableEnumKey addressableEnumKey, string labelString = null
        //     , Transform parent = null, bool instantiateInWorldSpace = false)
        // {
        //     var resourceLocation = GetResourceLocation(addressableEnumKey, null, labelString);
        //     return resourceLocation != null
        //         ? InstantiateInternal(resourceLocation, addressableEnumKey, parent, instantiateInWorldSpace)
        //         : null;
        // }
        //
        // #endregion
        //
        //
        //
        // #region Internal Utils
        //
        // private T LoadAssetInternal<T>(IResourceLocation resourceLocation, AddressableEnumKey addressableEnumKey)
        //     where T : Object
        // {
        //     var loadAssetHandle = Addressables.LoadAssetAsync<T>(resourceLocation);
        //     if (loadAssetHandle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         return loadAssetHandle.Result;
        //     }
        //
        //     DeLog.LogError($"Failed to load asset for key: {addressableEnumKey}");
        //     return null;
        // }
        //
        // private IResourceLocation GetResourceLocation(AddressableEnumKey addressableEnumKey
        //     , AssetLabelReference labelReference = null
        //     , string labelString = null)
        // {
        //     if (labelReference != null)
        //     {
        //         if (_coreManagementSystem.AssetConfigDataMap.TryGetValue(labelReference, out var addressableKeyMap))
        //         {
        //             if (addressableKeyMap.TryGetValue(addressableEnumKey, out var dataResourceLocation))
        //             {
        //                 return dataResourceLocation;
        //             }
        //
        //             DeLog.LogError($"Key not found: {addressableEnumKey}");
        //             return null;
        //         }
        //
        //         DeLog.LogError($"Label not found: {labelReference.labelString}");
        //         return null;
        //     }
        //
        //     if (string.IsNullOrEmpty(labelString))
        //     {
        //         return TryGetResourceLocationFromEntries(addressableEnumKey, _coreManagementSystem.AssetConfigDataMap);
        //     }
        //     
        //     var filteredEntries = _coreManagementSystem.AssetConfigDataMap
        //         .Where(entry => entry.Key.labelString == labelString);
        //
        //     return TryGetResourceLocationFromEntries(addressableEnumKey, filteredEntries);
        //
        // }
        //
        // private IResourceLocation TryGetResourceLocationFromEntries(AddressableEnumKey addressableEnumKey
        //     , IEnumerable<KeyValuePair<AssetLabelReference, Dictionary<AddressableEnumKey, IResourceLocation>>> entries)
        // {
        //     foreach (var entry in entries)
        //     {
        //         if (entry.Value.TryGetValue(addressableEnumKey, out var dataResourceLocation))
        //         {
        //             return dataResourceLocation;
        //         }
        //     }
        //
        //     DeLog.LogError($"Resource location with key {addressableEnumKey} not found.");
        //     return null;
        // }
        //
        // private GameObject InstantiateInternal(IResourceLocation resourceLocation,
        //     AddressableEnumKey addressableEnumKey,
        //     Transform parent, bool instantiateInWorldSpace)
        // {
        //     try
        //     {
        //         var instantiateHandle = Addressables.InstantiateAsync(resourceLocation, parent, instantiateInWorldSpace);
        //         var instance = instantiateHandle.WaitForCompletion();
        //         if (instance != null)
        //         {
        //             return instance;
        //         }
        //
        //         DeLog.LogError($"Failed to instantiate GameObject for key: {addressableEnumKey}");
        //         return null;
        //     }
        //     catch (Exception exception)
        //     {
        //         DeLog.LogError($"Exception occurred while instantiating GameObject for key: {addressableEnumKey}. Exception: {exception.Message}");
        //         return null;
        //     }
        // }
        //
        // #endregion
    }
}