
using UnityEditor;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Provides utility menu items for the ActFitFramework, specifically for managing Addressable system operations.
    /// Includes a menu option to force resolve issues by clearing cached configuration flags stored in EditorPrefs,
    /// helping to prevent potential crashes or inconsistent states during Addressable system operations.
    /// </summary>
    public static class AddressableUtilsMenu
    {
        #region Menu Item

        private const string PendingConfigGenerationKey = "ActFitFramework.PendingConfigGeneration";

        [MenuItem("ActFit/Addressables/[Crash Fixed] Force Resolve (Clear Cache)", priority = 99)]
        public static void ClearCachedEditorPref()
        {
            EditorPrefs.SetBool(PendingConfigGenerationKey, false);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("ActFit/Addressables/[Crash Fixed] Force Resolve (Clear USE KEY Symbol)", priority = 99)]
        public static void RemoveUseKeySymbol()
        {
            AddressableDefineSymbols.DisableUseActualKey();
            AssetDatabase.Refresh();
        }


        #endregion
    }
}