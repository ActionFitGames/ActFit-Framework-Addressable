
namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface IAssetProvider
    {
        /// <summary>
        /// Gets a non-GameObject asset of type T using an AddressableKey.
        /// </summary>
        /// <typeparam name="T">The type of asset to load. Must not be a GameObject.</typeparam>
        /// <param name="addressableKey">The key to the addressable asset.</param>
        /// <returns>The loaded asset of type T.</returns>
        T GetAsset<T>(AddressableKey addressableKey) where T : UnityEngine.Object;

        /// <summary>
        /// Instantiates a GameObject at a specified position and rotation, with an optional parent Transform.
        /// </summary>
        /// <param name="addressableKey">The key to the addressable GameObject.</param>
        /// <param name="position">The position to instantiate the GameObject.</param>
        /// <param name="rotation">The rotation to apply to the instantiated GameObject.</param>
        /// <param name="parent">The parent transform to attach the instantiated GameObject to (optional).</param>
        /// <returns>The instantiated GameObject.</returns>
        UnityEngine.GameObject Instantiate(AddressableKey addressableKey, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Transform parent = null);

        /// <summary>
        /// Instantiates a GameObject as a child of a specified parent Transform.
        /// </summary>
        /// <param name="addressableKey">The key to the addressable GameObject.</param>
        /// <param name="parent">The parent transform to attach the instantiated GameObject to.</param>
        /// <param name="instantiateInWorldSpace">Whether to instantiate the GameObject in world space.</param>
        /// <returns>The instantiated GameObject.</returns>
        UnityEngine.GameObject Instantiate(AddressableKey addressableKey, UnityEngine.Transform parent = null, bool instantiateInWorldSpace = false);
    }
}