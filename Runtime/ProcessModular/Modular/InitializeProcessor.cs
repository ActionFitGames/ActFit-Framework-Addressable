
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Initializes the Addressables system asynchronously and provides the resource locator.
    /// This class manages the initialization process and handles callbacks.
    /// </summary>
    internal class InitializeProcessor : IInitializeProcessor
    {
        private readonly ProcessCallbackSystem _processCallbackSystem;
        
        internal InitializeProcessor(ProcessCallbackSystem processCallbackSystem)
        {
            _processCallbackSystem = processCallbackSystem;
        }
        
        /// <summary>
        /// Initializes the Addressables system asynchronously.
        /// </summary>
        /// <param name="autoReleaseHandle">Determines whether the handle should be automatically released after initialization.</param>
        /// <returns>The resource locator for the initialized Addressables system.</returns>
        public async UniTask<IResourceLocator> Initialize(bool autoReleaseHandle = true)
        {
            var setting = AddressableMonoBehavior.Setting;
            
            try
            {
                var initializeHandle = Addressables.InitializeAsync(false);
                var initializeTask = initializeHandle.ToUniTask();

                var initializeAsyncResult = await initializeTask;
                _processCallbackSystem.CallbackInitialized(initializeAsyncResult
                    , () => {DeLog.Log("[Addressables] Initialize Async Completed");});

                if (!autoReleaseHandle)
                {
                    return initializeAsyncResult;
                }

                Addressables.Release(initializeHandle);
                return initializeAsyncResult;
            }
            catch (Exception exception)
            {
                DeLog.LogError("An error occurred during initialization.");
                DeLogHandler.DeLogException(exception, setting.GetExceptionType);
                return null;
            }
        }
    }
}