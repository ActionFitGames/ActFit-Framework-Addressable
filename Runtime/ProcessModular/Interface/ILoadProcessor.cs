
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ILoadProcessor
    {
        UniTask LoadAssetsAsync<T>(AssetLabelReference labelReference
            , Action<float> onProgress = null, Action<T> onCallbackLoaded = null, Action onCallbackCompleted = null) where T : Object;

        UniTask LoadAssetAsync<T>(string key
            , Action<float> onProgress = null, Action<T> onCallbackLoaded = null, Action onCallbackCompleted = null) where T : Object;
    }
}