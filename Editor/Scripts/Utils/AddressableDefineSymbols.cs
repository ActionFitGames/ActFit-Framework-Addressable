
using UnityEngine;
using UnityEditor;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public static class AddressableDefineSymbols
    {
        public const string DebugSymbol = "ADDRESSABLE_DEBUG";
        public const string UseActualKey = "ADDRESSABLE_USE_KEY";
        
        
        #region Symbol Setting Method
        private static void AddSymbol(string symbol)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            // 둘 중 하나라도 일치하지 않을 경우 알맞지 않는 심볼
            if (string.CompareOrdinal(symbol, DebugSymbol) != 0
                && string.CompareOrdinal(symbol, UseActualKey) != 0)
            {
                Debug.LogError($"'{symbol}' is mismatch 'Debug' or 'UseKey', ensure please.");
                return;
            }

            if (symbols.Contains(symbol))
            {
                return;
            }
            
            symbols += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
        }

        private static void RemoveSymbol(string symbol)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (string.CompareOrdinal(symbol, DebugSymbol) != 0
                && string.CompareOrdinal(symbol, UseActualKey) != 0)
            {
                Debug.LogError($"'{symbol}' is mismatch 'Debug' or 'UseKey', ensure please.");
                return;
            }

            if (!symbols.Contains(symbol))
            {
                return;
            }
            
            // Remove the symbol and clean up the string
            symbols = symbols.Replace(symbol, "").Replace(";;", ";").TrimEnd(';');
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
        }

        #endregion

        
        
        #region Enabled & Disabled (Access Methods)

        public static void EnableDebug() => AddSymbol(DebugSymbol);

        public static void DisableDebug() => RemoveSymbol(DebugSymbol);

        public static void EnableUseActualKey() => AddSymbol(UseActualKey);

        public static void DisableUseActualKey() => RemoveSymbol(UseActualKey);

        #endregion
    }
}