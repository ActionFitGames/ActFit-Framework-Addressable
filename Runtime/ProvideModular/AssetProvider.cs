
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class AssetProvider : IAssetProvider
    {
        private readonly AddressableSystem _addressableSystem;

        internal AssetProvider(AddressableSystem addressableSystem)
        {
            _addressableSystem = addressableSystem;
        }

        /// <summary>
        /// Gets a non-GameObject asset of type T using an AddressableKey.
        /// </summary>
        /// <typeparam name="T">The type of asset to load. Must not be a GameObject.</typeparam>
        /// <param name="addressableKey">The key to the addressable asset.</param>
        /// <returns>The loaded asset of type T, or null if the asset is not found or is of type GameObject.</returns>
        public T GetAsset<T>(AddressableKey addressableKey) where T : Object
        {
            if (typeof(T) == typeof(GameObject))
            {
                DeLog.LogError("GetAsset<T> cannot be used with GameObject type. Use Instantiate instead.");
                return null;
            }

            if (_addressableSystem.AssetHandleMap.TryGetValue(addressableKey, out var handle))
            {
                return handle.Result as T;
            }

            DeLog.LogError($"Asset of type {typeof(T)} not found for key: {addressableKey}");
            return null;
        }

        /// <summary>
        /// Instantiates a GameObject at a specified position and rotation, with an optional parent Transform.
        /// </summary>
        /// <param name="addressableKey">The key to the addressable GameObject.</param>
        /// <param name="position">The position to instantiate the GameObject.</param>
        /// <param name="rotation">The rotation to apply to the instantiated GameObject.</param>
        /// <param name="parent">The parent transform to attach the instantiated GameObject to (optional).</param>
        /// <returns>The instantiated GameObject.</returns>
        public GameObject Instantiate(AddressableKey addressableKey, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return InstantiateInternal(addressableKey, position, rotation, parent);
        }

        /// <summary>
        /// Instantiates a GameObject as a child of a specified parent Transform.
        /// </summary>
        /// <param name="addressableKey">The key to the addressable GameObject.</param>
        /// <param name="parent">The parent transform to attach the instantiated GameObject to.</param>
        /// <param name="instantiateInWorldSpace">Whether to instantiate the GameObject in world space.</param>
        /// <returns>The instantiated GameObject.</returns>
        public GameObject Instantiate(AddressableKey addressableKey, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return InstantiateInternal(addressableKey, Vector3.zero, Quaternion.identity, parent, instantiateInWorldSpace);
        }

        /// <summary>
        /// Internal method to instantiate a GameObject using the specified parameters.
        /// This method is used by both overloads of the Instantiate method.
        /// </summary>
        /// <param name="addressableKey">The key to the addressable GameObject.</param>
        /// <param name="position">The position to instantiate the GameObject.</param>
        /// <param name="rotation">The rotation to apply to the instantiated GameObject.</param>
        /// <param name="parent">The parent transform to attach the instantiated GameObject to.</param>
        /// <param name="instantiateInWorldSpace">Whether to instantiate the GameObject in world space.</param>
        /// <returns>The instantiated GameObject.</returns>
        private GameObject InstantiateInternal(AddressableKey addressableKey, Vector3 position, Quaternion rotation, Transform parent, bool instantiateInWorldSpace = false)
        {
            if (!_addressableSystem.AssetHandleMap.ContainsKey(addressableKey))
            {
                DeLog.LogError($"Resource not loaded or already released: {addressableKey}");
                return null;
            }

            if (!_addressableSystem.AssetKeyLocationMap.TryGetValue(addressableKey, out var resourceLocation))
            {
                DeLog.LogError($"Resource location not found for key: {addressableKey}");
                return null;
            }

            var handle = (parent != null) 
                ? Addressables.InstantiateAsync(resourceLocation, parent, instantiateInWorldSpace) 
                : Addressables.InstantiateAsync(resourceLocation, position, rotation, parent);
            handle.WaitForCompletion();

            if (!_addressableSystem.InstantiateAssetMap.ContainsKey(addressableKey))
            {
                _addressableSystem.InstantiateAssetMap[addressableKey] = new List<GameObject>();
            }

            _addressableSystem.InstantiateAssetMap[addressableKey].Add(handle.Result);

            return handle.Result;
        }
    }
}