
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Handles the release of assets managed by the AddressableSystem.
    /// This includes releasing assets based on AssetLabelReference, AddressableKey, 
    /// or directly releasing instances of loaded GameObjects.
    /// </summary>
    public class ReleaseProcessor : IReleaseProcessor
    {
        private readonly AddressableSystem _addressableSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseProcessor"/> class.
        /// </summary>
        /// <param name="addressableSystem">The addressable system instance to use for releasing assets.</param>
        public ReleaseProcessor(AddressableSystem addressableSystem)
        {
            _addressableSystem = addressableSystem;
        }

        /// <summary>
        /// Releases all assets associated with a given AssetLabelReference.
        /// </summary>
        /// <param name="assetLabelReference">The label reference identifying the assets to release.</param>
        public void Release(AssetLabelReference assetLabelReference)
        {
            if (assetLabelReference == null) return;

            if (_addressableSystem.LabelAssetHandleMap.TryGetValue(assetLabelReference.labelString, out var handles))
            {
                ReleaseHandles(handles);
                _addressableSystem.LabelAssetHandleMap.Remove(assetLabelReference.labelString);
            }
        }

        /// <summary>
        /// Releases all assets associated with a list of AssetLabelReferences.
        /// </summary>
        /// <param name="assetLabelReferences">The list of label references identifying the assets to release.</param>
        public void Release(List<AssetLabelReference> assetLabelReferences)
        {
            if (assetLabelReferences == null || assetLabelReferences.Count == 0) return;

            foreach (var labelReference in assetLabelReferences)
            {
                Release(labelReference);
            }
        }

        /// <summary>
        /// Releases an asset associated with a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the asset to release.</param>
        public void Release(AddressableKey addressableKey)
        {
            if (_addressableSystem.AssetHandleMap.TryGetValue(addressableKey, out var handle))
            {
                ReleaseHandle(addressableKey, handle);
            }
        }

        /// <summary>
        /// Releases all GameObject instances associated with a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the GameObject instances to release.</param>
        public void ReleaseAllInstances(AddressableKey addressableKey)
        {
            if (_addressableSystem.InstantiateAssetMap.TryGetValue(addressableKey, out var instances))
            {
                foreach (var instance in instances)
                {
                    if (instance != null)
                    {
                        Addressables.ReleaseInstance(instance);
                    }
                }
                _addressableSystem.InstantiateAssetMap.Remove(addressableKey);
            }
        }

        /// <summary>
        /// Releases a specific GameObject instance.
        /// </summary>
        /// <param name="instance">The specific GameObject instance to release.</param>
        public void ReleaseInstance(GameObject instance)
        {
            foreach (var kvp in _addressableSystem.InstantiateAssetMap)
            {
                if (kvp.Value.Remove(instance))
                {
                    Addressables.ReleaseInstance(instance);

                    // If the list is now empty, remove the key from the map
                    if (kvp.Value.Count == 0)
                    {
                        _addressableSystem.InstantiateAssetMap.Remove(kvp.Key);
                    }

                    break; // Exit the loop since the instance is found and removed
                }
            }
        }
        
        /// <summary>
        /// Releases all assets and associated GameObject instances for a given AssetLabelReference.
        /// </summary>
        /// <param name="assetLabelReference">The label reference identifying the assets to release.</param>
        public void ReleaseWithInstance(AssetLabelReference assetLabelReference)
        {
            if (assetLabelReference == null) return;

            if (_addressableSystem.LabelAssetHandleMap.TryGetValue(assetLabelReference.labelString, out var handles))
            {
                foreach (var handle in handles)
                {
                    ReleaseAllInstances(GetAddressableKey(handle));
                }

                ReleaseHandles(handles);
                _addressableSystem.LabelAssetHandleMap.Remove(assetLabelReference.labelString);
            }
        }

        /// <summary>
        /// Releases all assets and associated GameObject instances for a list of AssetLabelReferences.
        /// </summary>
        /// <param name="assetLabelReferences">The list of label references identifying the assets to release.</param>
        public void ReleaseWithInstance(List<AssetLabelReference> assetLabelReferences)
        {
            if (assetLabelReferences == null || assetLabelReferences.Count == 0) return;

            foreach (var labelReference in assetLabelReferences)
            {
                ReleaseWithInstance(labelReference);
            }
        }

        /// <summary>
        /// Releases all assets and associated GameObject instances for a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the asset and its instances to release.</param>
        public void ReleaseWithInstance(AddressableKey addressableKey)
        {
            ReleaseAllInstances(addressableKey);
            Release(addressableKey);
        }

        #region Internal

        /// <summary>
        /// Releases a list of AsyncOperationHandle instances.
        /// </summary>
        /// <param name="handles">The list of handles to release.</param>
        private void ReleaseHandles(IList<AsyncOperationHandle<Object>> handles)
        {
            foreach (var handle in handles)
            {
                ReleaseHandle(handle);
            }
        }

        /// <summary>
        /// Releases an asset associated with a specific AddressableKey and its corresponding handle.
        /// </summary>
        /// <param name="addressableKey">The key identifying the asset to release.</param>
        /// <param name="handle">The handle of the asset to release.</param>
        private void ReleaseHandle(AddressableKey addressableKey, AsyncOperationHandle<Object> handle)
        {
            foreach (var label in _addressableSystem.LabelAssetHandleMap.Keys)
            {
                var handles = _addressableSystem.LabelAssetHandleMap[label];
                if (!handles.Remove(handle) || handles.Count != 0)
                {
                    continue;
                }
                _addressableSystem.LabelAssetHandleMap.Remove(label);
                break;
            }

            _addressableSystem.AssetHandleMap.Remove(addressableKey);
            Addressables.Release(handle);
        }

        /// <summary>
        /// Releases an asset based on its AsyncOperationHandle.
        /// </summary>
        /// <param name="handle">The handle of the asset to release.</param>
        private void ReleaseHandle(AsyncOperationHandle<Object> handle)
        {
            foreach (var kvp in _addressableSystem.AssetHandleMap)
            {
                if (!kvp.Value.Equals(handle))
                {
                    continue;
                }
                
                _addressableSystem.AssetHandleMap.Remove(kvp.Key);
                break;
            }

            Addressables.Release(handle);
        }
        
        /// <summary>
        /// Utility method to retrieve the AddressableKey associated with a given AsyncOperationHandle.
        /// </summary>
        /// <param name="handle">The AsyncOperationHandle to search for.</param>
        /// <returns>The AddressableKey if found, otherwise null.</returns>
        private AddressableKey GetAddressableKey(AsyncOperationHandle<Object> handle)
        {
            foreach (var kvp in _addressableSystem.AssetHandleMap)
            {
                if (kvp.Value.Equals(handle))
                {
                    return kvp.Key;
                }
            }
            
            return default;
        }

        #endregion
    }
}