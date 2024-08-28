
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal class AddressableMonoBehavior : MonoBehaviour
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

            var gameObj = new GameObject("Addressable System");
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
                foreach (var labelReferenceString in Cache.GetLabelStrings)
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
        [UnityEditor.InitializeOnLoadMethod]
        private static void SetCustomIcon()
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            
            var iconPath = "Packages/ActionFit-Standalone Addressables/Editor/Assets/ActFitFrameworkIcon.png";
            var icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon != null)
            {
                UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += (instanceID, selectionRect) =>
                {
                    var obj = UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                    if (obj != null && obj.GetComponent<AddressableMonoBehavior>() != null)
                    {
                        // 이 위치에 아이콘을 그립니다. 기본 아이콘 영역에 가깝게 조정할 수 있습니다.
                        var iconRect = new Rect(selectionRect.xMin - 21, selectionRect.yMin - 1, 20, 18);
                        GUI.DrawTexture(iconRect, icon);
                    }
                };
            }
            else
            {
                Debug.LogError($"Icon not found at path: {iconPath}");
            }
        }
#endif
    }
}
