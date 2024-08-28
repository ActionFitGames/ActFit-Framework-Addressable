
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// The AddressableSystem class manages the loading, releasing, and providing of addressable assets.
    /// It implements the Singleton pattern to ensure only one instance is active.
    /// </summary>
    public class AddressableSystem
    {
        #region Fields

        /// <summary> Singleton Instance </summary>
        private static volatile AddressableSystem _sInstance = null;
        private static readonly object LockObject = new();

        /// <summary> Async Operation Handles for assets (Release Overlap) </summary>
        internal readonly Dictionary<AddressableKey, AsyncOperationHandle<Object>> AssetHandleMap = new();
        internal readonly Dictionary<string, IList<AsyncOperationHandle<Object>>> LabelAssetHandleMap = new();

        /// <summary> Mapping of resource locations by asset key and label </summary>
        internal readonly Dictionary<string, IList<ResourceKvp>> LabelAssetKeyLocationMap = new();
        internal readonly Dictionary<AddressableKey, IResourceLocation> AssetKeyLocationMap = new();

        /// <summary> Mapping of instantiate async GameObject with addressableKey </summary>
        internal readonly Dictionary<AddressableKey, IList<GameObject>> InstantiateAssetMap = new();

        /// <summary> Processor and Asset Provider </summary>
        private readonly IAssetProvider _assetProvider;
        internal IProcessorProvider _processorProvider;
        internal bool IsInitialize;

        #endregion

        /// <summary>
        /// Private constructor for the AddressableSystem. Initializes processors and asset provider.
        /// </summary>
        private AddressableSystem()
        {
            var processCallbackSystem = new ProcessCallbackSystem(this);
            _assetProvider = new AssetProvider(this);
            GetProcessor(this, processCallbackSystem);
        }
        
        /// <summary>
        /// Initializes the processor provider with the addressable system and callback system.
        /// </summary>
        /// <param name="addressableSystem">The AddressableSystem instance.</param>
        /// <param name="processCallbackSystem">The callback system for processing events and state changes.</param>
        private void GetProcessor(AddressableSystem addressableSystem, ProcessCallbackSystem processCallbackSystem)
        {
            _processorProvider = ProcessFactory.GetAddressableProcessorProvider(addressableSystem, processCallbackSystem);
        }

        #region Singleton

        /// <summary>
        /// Gets the singleton instance of the AddressableSystem.
        /// </summary>
        public static AddressableSystem Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (_sInstance != null)
                    {
                        return _sInstance;
                    }
                    
                    DeLog.Log("[Addressable System] Initializing the instance");
                    _sInstance = new AddressableSystem();
                    return _sInstance;
                }
            }
        }

        /// <summary>
        /// Activates the AddressableSystem, ensuring it is initialized and ready for use.
        /// </summary>
        /// <returns>The singleton instance of the AddressableSystem.</returns>
        public static AddressableSystem Activate()
        {
            DeLog.Log($"[Addressable System] Activate.");
            return Instance;
        }

        #endregion

        /// <summary>
        /// Gets the load processor for managing asset loading operations.
        /// </summary>
        /// <returns>An instance of ILoadProcessor.</returns>
        public ILoadProcessor GetLoadProcessor()
        {
            return _processorProvider.GetLoader();
        }

        /// <summary>
        /// Gets the release processor for managing asset release operations.
        /// </summary>
        /// <returns>An instance of IReleaseProcessor.</returns>
        public IReleaseProcessor GetReleaseProcessor()
        {
            return _processorProvider.GetReleaser();
        }

        /// <summary>
        /// Gets the asset provider for managing access to addressable assets.
        /// </summary>
        /// <returns>An instance of IAssetProvider.</returns>
        public IAssetProvider GetAssetProvider()
        {
            return _assetProvider;
        }
    }
}