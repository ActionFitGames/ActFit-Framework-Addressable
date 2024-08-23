
using System;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public static class DeLog
    {
        public static void Log(string message)
        {
#if ADDRESSABLE_DELOG_ENABLE
        Debug.Log(message);
#endif
        }

        public static void LogWarning(string message)
        {
#if ADDRESSABLE_DELOG_ENABLE
        Debug.LogWarning(message);
#endif
        }

        public static void LogError(string message)
        {
#if ADDRESSABLE_DELOG_ENABLE
        Debug.LogError(message);
#endif
        }
    
        public static void LogException(Exception exception)
        {
#if ADDRESSABLE_DELOG_ENABLE
        Debug.LogException(exception);
#endif
        } 
    }
}