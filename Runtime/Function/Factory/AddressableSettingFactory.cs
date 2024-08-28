
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal static class AddressableSettingFactory
    {
        private const string ResourcePath = "ScriptableObjects/Addressables/AddressableSystemSettingConvert";
        
        public static ISettingProvider GetSetting()
        {
            var setting = Resources.Load<AddressableSettingConvertSO>(ResourcePath);

            if (!setting)
            {
                Debug.LogError($"[Addressable System] Setting not found at {ResourcePath}.");
                return null;
            }

            return setting;
        }
    }
}