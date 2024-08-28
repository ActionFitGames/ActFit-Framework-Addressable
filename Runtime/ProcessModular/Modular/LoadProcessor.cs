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
        private AddressableSystem _addressableSystem;

        internal LoadProcessor(AddressableSystem addressableSystem)
        {
            _addressableSystem = addressableSystem;
        }

        #region Load Asset - Multiple(s)

        public async UniTask LoadAssetsAsync<T>(AssetLabelReference labelReference
            , Action<float> onProgress = null
            , Action<T> onCallbackLoaded = null
            , Action onCallbackCompleted = null) where T : Object
        {
            await UniTask.WaitUntil(() => _addressableSystem.IsInitialize);

            if (labelReference == null)
            {
                DeLogHandler.DeLogInvalidLabelException(AddressableMonoBehavior.Setting.GetExceptionType);
                return;
            }

            try
            {
                if (_addressableSystem.LoadedLabelHandlesMap.TryGetValue(labelReference, out var existingHandleObj))
                {
                    // 이미 로드된 경우
                    DeLog.Log($"Already loaded '{labelReference.labelString}'");
                    var existingHandle = (AsyncOperationHandle<IList<T>>)existingHandleObj;
                    InvokeExistingAssets(existingHandle, onCallbackLoaded);
                    onCallbackCompleted?.Invoke();
                    return;
                }

                if (!_addressableSystem.LabelLocationMap.TryGetValue(labelReference.labelString, out var resourceLocations))
                {
                    DeLog.LogError($"No resource locations found for the given label. '{labelReference}'");
                    return;
                }

                var loadAssetsHandle = Addressables.LoadAssetsAsync<T>(resourceLocations, asset => { onCallbackLoaded?.Invoke(asset); });

                _addressableSystem.LoadedLabelHandlesMap[labelReference] = loadAssetsHandle;

                while (!loadAssetsHandle.IsDone)
                {
                    onProgress?.Invoke(loadAssetsHandle.PercentComplete);
                    await UniTask.Yield();
                }

                await loadAssetsHandle.ToUniTask();
                await UniTask.Yield();
                onProgress?.Invoke(loadAssetsHandle.PercentComplete);

                onCallbackCompleted?.Invoke();
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
            }
        }

        private void InvokeExistingAssets<T>(AsyncOperationHandle<IList<T>> existingHandle, Action<T> onCallbackLoaded) where T : Object
        {
            if (existingHandle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var asset in existingHandle.Result)
                {
                    onCallbackLoaded?.Invoke(asset);
                }
            }
        }

        #endregion

        #region Load Asset - Singly
        
        public async UniTask LoadAssetAsync<T>(AddressableKey key
            , Action<float> onProgress = null
            , Action<T> onCallbackLoaded = null
            , Action onCallbackCompleted = null) where T : Object
        {
            await UniTask.WaitUntil(() => _addressableSystem.IsInitialize);

            try
            {
                if (_addressableSystem.LoadedKeyHandlesMap.TryGetValue(key, out var existingHandleObj))
                {
                    // 이미 로드된 경우
                    DeLog.Log($"Already loaded '{key}'");
                    var existingHandle = (AsyncOperationHandle<T>)existingHandleObj;
                    onCallbackLoaded?.Invoke(existingHandle.Result);
                    onCallbackCompleted?.Invoke();
                    return;
                }

                if (!_addressableSystem.KeyLocationMap.TryGetValue(key, out var resourceLocation))
                {
                    DeLog.LogError($"No resource locations found for the given AddressableKey. '{key}'");
                    return;
                }

                var loadAssetHandle = Addressables.LoadAssetAsync<T>(resourceLocation);
                _addressableSystem.LoadedKeyHandlesMap[key] = loadAssetHandle;

                while (!loadAssetHandle.IsDone)
                {
                    onProgress?.Invoke(loadAssetHandle.PercentComplete);
                    await UniTask.Yield();
                }

                var asset = await loadAssetHandle.ToUniTask();
                await UniTask.Yield();
                onProgress?.Invoke(loadAssetHandle.PercentComplete);

                onCallbackLoaded?.Invoke(asset);
                onCallbackCompleted?.Invoke();
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
            }
        }

        public async UniTask LoadAssetAsync<T>(string key
            , Action<float> onProgress = null
            , Action<T> onCallbackLoaded = null
            , Action onCallbackCompleted = null) where T : Object
        {
            await UniTask.WaitUntil(() => _addressableSystem.IsInitialize);

            if (string.IsNullOrEmpty(key))
            {
                DeLog.LogError("Invalid key provided.");
                return;
            }

            try
            {
                if (_addressableSystem.LoadedStringKeyHandlesMap.TryGetValue(key, out var existingHandleObj))
                {
                    // 이미 로드된 경우
                    DeLog.Log($"Already loaded '{key}'");
                    var existingHandle = (AsyncOperationHandle<T>)existingHandleObj;
                    onCallbackLoaded?.Invoke(existingHandle.Result);
                    onCallbackCompleted?.Invoke();
                    return;
                }

                var loadAssetHandle = Addressables.LoadAssetAsync<T>(key);
                _addressableSystem.LoadedStringKeyHandlesMap[key] = loadAssetHandle;

                while (!loadAssetHandle.IsDone)
                {
                    onProgress?.Invoke(loadAssetHandle.PercentComplete);
                    await UniTask.Yield();
                }

                var asset = await loadAssetHandle.ToUniTask();
                await UniTask.Yield();
                onProgress?.Invoke(loadAssetHandle.PercentComplete);

                onCallbackLoaded?.Invoke(asset);
                onCallbackCompleted?.Invoke();
            }
            catch (Exception exception)
            {
                DeLogHandler.DeLogException(exception, AddressableMonoBehavior.Setting.GetExceptionType);
            }
        }

        #endregion
    }
}