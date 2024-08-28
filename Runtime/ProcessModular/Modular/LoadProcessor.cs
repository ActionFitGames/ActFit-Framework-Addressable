
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal class LoadProcessor : ILoadProcessor
    {
        private readonly AddressableSystem _addressableSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadProcessor"/> class.
        /// </summary>
        /// <param name="addressableSystem">The addressable system instance to use for loading assets.</param>
        internal LoadProcessor(AddressableSystem addressableSystem)
        {
            _addressableSystem = addressableSystem;
        }

        /// <summary>
        /// Asynchronously loads assets based on a single AssetLabelReference.
        /// Provides progress updates and invokes callbacks when assets are loaded and completed.
        /// </summary>
        /// <param name="labelReference">The label reference identifying the assets to load.</param>
        /// <param name="onProgress">Callback to invoke with the loading progress (0 to 1).</param>
        /// <param name="onCallbackLoaded">Callback to invoke when an asset is loaded.</param>
        /// <param name="onCallbackCompleted">Callback to invoke when all assets are loaded.</param>
        public async UniTask LoadAssetsAsync(AssetLabelReference labelReference
            , Action<float> onProgress = null
            , Action<Object> onCallbackLoaded = null
            , Action onCallbackCompleted = null)
        {
            if (labelReference == null)
            {
                DeLogHandler.DeLogInvalidLabelException(AddressableMonoBehavior.Setting.GetExceptionType);
                return;
            }

            await UniTask.WaitUntil(() => _addressableSystem.IsInitialize);
            await LoadAssetsAsyncInternal(new List<AssetLabelReference> { labelReference }, onProgress, onCallbackLoaded, onCallbackCompleted);
        }

        /// <summary>
        /// Asynchronously loads assets based on a list of AssetLabelReferences.
        /// Provides progress updates and invokes callbacks when assets are loaded and completed.
        /// </summary>
        /// <param name="labelReferences">The list of label references identifying the assets to load.</param>
        /// <param name="onProgress">Callback to invoke with the loading progress (0 to 1).</param>
        /// <param name="onCallbackLoaded">Callback to invoke when an asset is loaded.</param>
        /// <param name="onCallbackCompleted">Callback to invoke when all assets are loaded.</param>
        public async UniTask LoadAssetsAsync(List<AssetLabelReference> labelReferences
            , Action<float> onProgress = null
            , Action<Object> onCallbackLoaded = null
            , Action onCallbackCompleted = null)
        {
            if (labelReferences == null || labelReferences.Count == 0)
            {
                DeLogHandler.DeLogInvalidLabelException(AddressableMonoBehavior.Setting.GetExceptionType);
                return;
            }

            await UniTask.WaitUntil(() => _addressableSystem.IsInitialize);
            await LoadAssetsAsyncInternal(labelReferences, onProgress, onCallbackLoaded, onCallbackCompleted);
        }

        /// <summary>
        /// Internal method that handles the loading of assets based on a list of label references.
        /// Tracks progress and invokes callbacks as assets are loaded.
        /// </summary>
        /// <param name="labelReferences">The list of label references identifying the assets to load.</param>
        /// <param name="onProgress">Callback to invoke with the overall loading progress (0 to 1).</param>
        /// <param name="onCallbackLoaded">Callback to invoke when an asset is loaded.</param>
        /// <param name="onCallbackCompleted">Callback to invoke when all assets are loaded.</param>
        private async UniTask LoadAssetsAsyncInternal(List<AssetLabelReference> labelReferences
            , Action<float> onProgress = null
            , Action<Object> onCallbackLoaded = null
            , Action onCallbackCompleted = null)
        {
            try
            {
                int totalResourcesToLoad = 0;
                foreach (var labelReference in labelReferences)
                {
                    if (_addressableSystem.LabelAssetKeyLocationMap.TryGetValue(labelReference.labelString, out var resourceKvps))
                    {
                        totalResourcesToLoad += resourceKvps.Count;
                    }
                }

                int loadedResourceCount = 0;
                foreach (var labelReference in labelReferences)
                {
                    if (!_addressableSystem.LabelAssetKeyLocationMap.TryGetValue(labelReference.labelString, out var resourceKvps))
                    {
                        continue;
                    }

                    if (!_addressableSystem.LabelAssetHandleMap.ContainsKey(labelReference.labelString))
                    {
                        _addressableSystem.LabelAssetHandleMap[labelReference.labelString] =
                            new List<AsyncOperationHandle<Object>>();
                    }
                    
                    foreach (var resourceKvp in resourceKvps)
                    {
                        try
                        {
                            await LoadAssetAsync(resourceKvp
                                , labelReference
                                , progress =>
                            {
                                var overallProgress = (float)loadedResourceCount / totalResourcesToLoad + progress / totalResourcesToLoad;
                                onProgress?.Invoke(overallProgress);
                            }
                                , onCallbackLoaded
                                , () =>
                            {
                                loadedResourceCount++;
                                if (loadedResourceCount == totalResourcesToLoad)
                                {
                                    onCallbackCompleted?.Invoke();
                                }
                            });
                        }
                        catch (InvalidOperationException ioEx)
                        {
                            DeLogHandler.DeLogException(ioEx, AddressableMonoBehavior.Setting.GetExceptionType);
                        }
                        catch (Exception exception)
                        {
                            DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
            }
        }

        /// <summary>
        /// Loads a single asset asynchronously based on a resource key-value pair.
        /// Tracks progress and invokes callbacks when the asset is loaded.
        /// </summary>
        /// <param name="resourceKvp">The key-value pair representing the resource to load.</param>
        /// <param name="labelKey">The label reference associated with the resource.</param>
        /// <param name="onProgress">Callback to invoke with the loading progress of the asset (0 to 1).</param>
        /// <param name="onCallbackLoaded">Callback to invoke when the asset is loaded.</param>
        /// <param name="onCallbackCompleted">Callback to invoke when the asset has finished loading.</param>
        private async UniTask LoadAssetAsync(ResourceKvp resourceKvp
            , AssetLabelReference labelKey
            , Action<float> onProgress = null
            , Action<Object> onCallbackLoaded = null
            , Action onCallbackCompleted = null)
        {
            try
            {
                if (_addressableSystem.AssetHandleMap.TryGetValue(resourceKvp.AddressableKey, out var handle))
                {
                    DeLog.Log($"Already Loaded Assets : {resourceKvp.AddressableKey.ToString()}");
                    onCallbackLoaded?.Invoke(handle.Result);
                    onCallbackCompleted?.Invoke();
                    return;
                }

                var loadAssetHandle = Addressables.LoadAssetAsync<Object>(resourceKvp.ResourceLocation);

                while (!loadAssetHandle.IsDone)
                {
                    onProgress?.Invoke(loadAssetHandle.PercentComplete);
                    await UniTask.Yield();
                }

                await loadAssetHandle.ToUniTask();
                onProgress?.Invoke(loadAssetHandle.PercentComplete);

                _addressableSystem.AssetHandleMap.TryAdd(resourceKvp.AddressableKey, loadAssetHandle);
                _addressableSystem.LabelAssetHandleMap[labelKey.labelString].Add(loadAssetHandle);
                
                onCallbackLoaded?.Invoke(loadAssetHandle.Result);
                onCallbackCompleted?.Invoke();
            }
            catch (InvalidOperationException ioEx)
            {
                DeLogHandler.DeLogException(ioEx, AddressableMonoBehavior.Setting.GetExceptionType);
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
            }
        }
    }
}