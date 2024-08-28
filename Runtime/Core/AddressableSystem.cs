using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class AddressableSystem
    {
        #region Fields

        /// <summary> Singleton Instance </summary>
        private static volatile AddressableSystem _sInstance = null;

        /// <summary> Async Operation Handles (Release Overlap) </summary>
        internal readonly Dictionary<AssetLabelReference, object> LoadedLabelHandlesMap = new();
        internal readonly Dictionary<AddressableKey, object> LoadedKeyHandlesMap = new();
        internal readonly Dictionary<string, object> LoadedStringKeyHandlesMap = new(); // For string keys

        /// <summary> IResourceLocation Data Mapping with KEY and LABEL (string) </summary>
        internal readonly Dictionary<string, IList<IResourceLocation>> LabelLocationMap = new();
        internal readonly Dictionary<AddressableKey, IResourceLocation> KeyLocationMap = new();

        /// <summary> Processor (Loader and Releaser) </summary>
        internal IProcessorProvider _processorProvider;
        internal bool IsInitialize;

        #endregion

        private AddressableSystem()
        {
            var processCallbackSystem = new ProcessCallbackSystem(this);
            GetProcessor(this, processCallbackSystem);
        }

        /// <summary>
        /// Create ProcessorProvider and MonoBehavior
        /// </summary>
        /// <param name="addressableSystem">this Instance.</param>
        /// <param name="processCallbackSystem">this Instance Callback system.</param>
        private void GetProcessor(AddressableSystem addressableSystem, ProcessCallbackSystem processCallbackSystem)
        {
            _processorProvider = ProcessFactory.GetAddressableProcessorProvider(addressableSystem, processCallbackSystem);
        }

        #region Singleton

        public static AddressableSystem Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    DeLog.Log("[Addressable System] Initializing the instance");
                    _sInstance = new AddressableSystem();
                }

                return _sInstance;
            }
        }

        public static AddressableSystem Activate()
        {
            DeLog.Log($"[Addressable System] Activate.");
            return Instance;
        }

        #endregion

        public ILoadProcessor GetLoadProcessor()
        {
            return _processorProvider.GetLoader();
        }
    }
}