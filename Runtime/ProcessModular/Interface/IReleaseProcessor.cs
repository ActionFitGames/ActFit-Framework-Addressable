
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface IReleaseProcessor
    {
        /// <summary>
        /// Releases all assets associated with a given AssetLabelReference.
        /// </summary>
        /// <param name="assetLabelReference">The label reference identifying the assets to release.</param>
        void Release(AssetLabelReference assetLabelReference);

        /// <summary>
        /// Releases all assets associated with a list of AssetLabelReferences.
        /// </summary>
        /// <param name="assetLabelReferences">The list of label references identifying the assets to release.</param>
        void Release(List<AssetLabelReference> assetLabelReferences);

        /// <summary>
        /// Releases an asset associated with a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the asset to release.</param>
        void Release(AddressableKey addressableKey);

        /// <summary>
        /// Releases a specific GameObject instance.
        /// </summary>
        /// <param name="instance">The specific GameObject instance to release.</param>
        void ReleaseInstance(GameObject instance);

        /// <summary>
        /// Releases all GameObject instances associated with a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the GameObject instances to release.</param>
        void ReleaseAllInstances(AddressableKey addressableKey);

        /// <summary>
        /// Releases all assets and associated GameObject instances for a given AssetLabelReference.
        /// </summary>
        /// <param name="assetLabelReference">The label reference identifying the assets to release.</param>
        void ReleaseWithInstance(AssetLabelReference assetLabelReference);

        /// <summary>
        /// Releases all assets and associated GameObject instances for a list of AssetLabelReferences.
        /// </summary>
        /// <param name="assetLabelReferences">The list of label references identifying the assets to release.</param>
        void ReleaseWithInstance(List<AssetLabelReference> assetLabelReferences);

        /// <summary>
        /// Releases all assets and associated GameObject instances for a specific AddressableKey.
        /// </summary>
        /// <param name="addressableKey">The key identifying the asset and its instances to release.</param>
        void ReleaseWithInstance(AddressableKey addressableKey);
    }
}