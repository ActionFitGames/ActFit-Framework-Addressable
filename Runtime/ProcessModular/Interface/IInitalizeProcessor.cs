
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface IInitializeProcessor
    {
        UniTask<IResourceLocator> Initialize(bool autoReleaseHandle = true);
    }
}