
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class AddressableMonoBehavior : MonoBehaviour
    {
        #region Fields
        
        // out (singleton) instance object
        private static AddressableMonoBehavior _instance;
        
        internal static ISettingProvider Setting;
        internal static ICacheProvider Cache;

        #endregion

        private void Awake()
        {
            Setting = AddressableSettingFactory.GetSetting();
            Cache = AddressableCacheFactory.GetCache();

            if (Setting.GetDontLoadConfig)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        /// <summary>
        /// Call this once from the GameThread.
        /// Create MonoBehavior Singleton class.
        /// </summary>
        internal static void CreateAddressableMonoBehavior()
        {
            if (_instance)
            {
                return;
            }

            var gameObj = new GameObject("SYSTEM [Addressables]");
            _instance = gameObj.AddComponent<AddressableMonoBehavior>();
        }

        internal static async UniTask ActivateFromInitializeAsync(IInitializeProcessor initializeProcessor
            , ILocationProcessor locationProcessor
            , Action<bool> onInitializeInvoker)
        {
            try
            {
                await initializeProcessor.Initialize();
            }
            catch (Exception exception)
            {
                onInitializeInvoker.Invoke(false);
                DeLog.LogError($"[Initialize Processor] Initialize Failed. {exception.Message}");
                return;
            }


            try
            {
                foreach (var labelReferenceString in Cache.GetLabelReferencesString)
                {
                    await locationProcessor.LoadLocationsAsync(labelReferenceString);
                }
            }
            catch (Exception exception)
            {
                onInitializeInvoker.Invoke(false);
                DeLog.LogError($"[Location Processor] Load Failed. {exception.Message}");
                return;
            }

            onInitializeInvoker.Invoke(true);
        }

#if UNITY_EDITOR
        
#endif
    }
}
