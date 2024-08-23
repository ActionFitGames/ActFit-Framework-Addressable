
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActFitFramework.Standalone.AddressableSystem
{
    public sealed class AddressableManagementSystem : MonoBehaviour
    {
        #region Fields

        // our (singleton) instance
        private static AddressableManagementSystem _instance;

        public static void CreateInstance()
        {
            if (_instance != null)
            {
                return;
            }
            
            
        }

        private const string CacheSOPath = "ScriptableObjects/Addressables/AddressableCache";
        internal AddressableCacheSO CacheSO;

        private ProcessForUniTask _processModuleForUniTask;

        [Header("Debug Config")] 
        public ExceptionHandleTypes ExceptionHandleType = ExceptionHandleTypes.Log;
        public bool IsDebuggingProcess;

        #endregion
        


        #region Unity Behavior
        
        private void Awake()
        {
            _instance = this;
            
            ResourceLoadCache();
            
            void ResourceLoadCache()
            {
                try
                {
                    CacheSO = Resources.Load<AddressableCacheSO>(CacheSOPath);
                }
                catch (Exception exception)
                {
                    DeLog.LogError($"[Resources] Can't loaded cached SO {exception.Message}");
                }
            }
        }
        
        #endregion



        #region Initialize
        
        public async UniTask Initialize()
        {
            
        }

        #endregion
    }
}