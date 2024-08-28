
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ILoadProcessor
    {
        UniTask LoadAssetsAsync(AssetLabelReference labelReference
            , Action<float> onProgress = null, Action<Object> onCallbackLoaded = null,
            Action onCallbackCompleted = null);

        UniTask LoadAssetsAsync(List<AssetLabelReference> labelReferences
            , Action<float> onProgress = null, Action<Object> onCallbackLoaded = null,
            Action onCallbackCompleted = null);
    }
}