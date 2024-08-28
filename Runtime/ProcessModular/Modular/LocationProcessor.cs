
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Handles loading of resource locations associated with specific asset labels.
    /// This class provides asynchronous methods to load and manage resource locations using Addressables.
    /// </summary>
    public class LocationProcessor : ILocationProcessor
    {
        private readonly ProcessCallbackSystem _processCallbackSystem;
        
        internal LocationProcessor(ProcessCallbackSystem processCallbackSystem)
        {
            _processCallbackSystem = processCallbackSystem;
        }
        
        /// <summary>
        /// Loads resource locations asynchronously based on the given label reference and type.
        /// </summary>
        /// <param name="labelReference">The label reference to load locations for.</param>
        /// <param name="type">The type of resources to filter by (optional).</param>
        /// <returns>A list of resource locations associated with the label.</returns>
        public async UniTask<IList<IResourceLocation>> LoadLocationsAsync(string labelReferenceString, Type type = null)
        {
            if (labelReferenceString == null)
            {
                DeLogHandler.DeLogInvalidLabelException(AddressableMonoBehavior.Setting.GetExceptionType);
                return null;
            }
            
            try
            {
                var loadLocationHandle = Addressables.LoadResourceLocationsAsync(labelReferenceString, type).ToUniTask();
                var loadLocationResult = await loadLocationHandle;
        
                _processCallbackSystem.CallbackLocationsLoaded(loadLocationResult, labelReferenceString);
                return loadLocationResult;
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
                return null;
            }
        }
    }
}