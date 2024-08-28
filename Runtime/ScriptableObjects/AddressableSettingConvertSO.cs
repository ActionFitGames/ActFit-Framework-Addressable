
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    // [CreateAssetMenu(fileName = "AddressableSettingConvertSO", menuName = "ActFit/Addressable Setting Convert SO")]
    public class AddressableSettingConvertSO : ScriptableObject, ISettingProvider
    {
        private ExceptionHandleTypes exceptionHandleType;
        private bool useDontLoadConfig;
        
        public ExceptionHandleTypes GetExceptionType => exceptionHandleType;
        public bool GetDontLoadConfig => useDontLoadConfig;
        
#if UNITY_EDITOR
        public void SetExceptionType(ExceptionHandleTypes type)
        {
            exceptionHandleType = type;
        }

        public void SetDontLoadConfig(bool value)
        {
            useDontLoadConfig = value;
        }
#endif
    }
}