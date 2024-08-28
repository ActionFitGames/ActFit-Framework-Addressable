
using System;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public enum ExceptionHandleTypes
    {
        Log,
        Throw,
        SuppressObsolete
    }
    
    internal static class DeLogHandler
    {
        internal static void DeLogException(Exception exception, ExceptionHandleTypes handleType)
        {
            switch (handleType)
            {
                case ExceptionHandleTypes.Log:
                    DeLog.LogException(exception);
                    break;
                case ExceptionHandleTypes.Throw:
                    throw exception;
                case ExceptionHandleTypes.SuppressObsolete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(handleType), handleType, null);
            }
        }

        internal static void DeLogInvalidLabelException(ExceptionHandleTypes handleType)
        {
            switch (handleType)
            {
                case ExceptionHandleTypes.Log:
                    DeLog.LogException(new InvalidKeyException((AssetLabelReference)null));
                    break;
                case ExceptionHandleTypes.Throw:
                    throw new InvalidKeyException((AssetLabelReference)null);
                case ExceptionHandleTypes.SuppressObsolete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(handleType), handleType, null);
            }
        }

        internal static void DeLogAllAssets<T>(T loadedAsset) where T : Object
        {
            DeLog.Log($"[{loadedAsset.name}' is Loaded\n");
        }
    }
}