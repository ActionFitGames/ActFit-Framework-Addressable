
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal static class AddressableCacheFactory
    {
        private const string ResourcePath = "ScriptableObjects/Addressables/AddressableCache";

        public static ICacheProvider GetCache()
        {
            var cache = Resources.Load<AddressableCacheSO>(ResourcePath);

            if (!cache)
            {
                Debug.LogError($"[Addressable System] Cache not found at {ResourcePath}.");
                return null;
            }

            return cache;
        }
    }
}
