
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// ScriptableObject used to cache addressable asset locations for faster access.
    /// This class maintains mappings between asset labels and their associated resource locations,
    /// as well as mappings between specific addressable keys and their resource locations.
    /// </summary>
    // [CreateAssetMenu(fileName = "AddressableCache", menuName = "ActFit/Addressables/CacheSO")]
    public class AddressableCacheSO : ScriptableObject, ICacheProvider
    {
        #region Fields
        
#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
        [SerializeField] [TextArea] private string _description = "Addressable IResourceLocation caching internalID\n" +
#pragma warning restore CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
                                                                  "  -- This is generate Enum 'AddressableKey'\n" +
                                                                  "  -- Runtime, Enum Key Mapping to IResourceLocation\n";
        
        [ReadOnly] public List<string> LabelStrings;
        
        [SerializedDictionary("[NoEdit] Unique Composite Key", "[Const] AddrKey")]
        [ReadOnly] public SerializedDictionary<string, int> AssetKeysMap;
        
        /// <summary> Implements 'ICacheProvider' </summary>
        public List<string> GetLabelStrings => LabelStrings;
        public Dictionary<string, int> GetAssetKeysMap => AssetKeysMap;

        #endregion

#if UNITY_EDITOR
        // Custom PropertyDrawer to make fields read-only in Inspector
        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false; // Disable editing
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;  // Re-enable editing
            }
        }

        public class ReadOnlyAttribute : PropertyAttribute { }
#endif
    }
}