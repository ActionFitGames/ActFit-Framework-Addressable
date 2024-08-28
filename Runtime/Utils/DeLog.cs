
using System;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public static class DeLog
    {
        public static void Log(string message)
        {
#if ADDRESSABLE_DEBUG
        Debug.Log(message);
#endif
        }

        public static void LogWarning(string message)
        {
#if ADDRESSABLE_DEBUG
        Debug.LogWarning(message);
#endif
        }

        public static void LogError(string message)
        {
#if ADDRESSABLE_DEBUG
        Debug.LogError(message);
#endif
        }
    
        public static void LogException(Exception exception)
        {
#if ADDRESSABLE_DEBUG
        Debug.LogException(exception);
#endif
        } 
    }
}