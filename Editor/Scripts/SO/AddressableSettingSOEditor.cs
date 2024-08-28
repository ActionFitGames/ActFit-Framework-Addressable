using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    [CustomEditor(typeof(AddressableSettingSO))]
    public class AddressableSettingSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _exceptionHandleTypeProp;
        private SerializedProperty _isDebugProp;
        private SerializedProperty _useDontDestroyOnLoadProp;
        private SerializedProperty _isAutoUpdateProp;

        private Texture2D _titleImage;

        private void OnEnable()
        {
            _exceptionHandleTypeProp = serializedObject.FindProperty("_exceptionHandleType");
            _isDebugProp = serializedObject.FindProperty("_isDebug");
            _useDontDestroyOnLoadProp = serializedObject.FindProperty("UseDontDestroyOnLoad");
            _isAutoUpdateProp = serializedObject.FindProperty("IsAutoUpdate");

            LoadTitleImage();
        }

        private void LoadTitleImage()
        {
            var assetPath = "Packages/ActFit-Framework-Addressable/Editor/Assets/ActFitFrameworkTitle.png";

            _titleImage = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTitleLabel();
            
            EditorGUILayout.PropertyField(_exceptionHandleTypeProp);
            EditorGUILayout.PropertyField(_isDebugProp);
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_useDontDestroyOnLoadProp);
            EditorGUILayout.HelpBox(
                "This toggle checked.\nAddressable System 'DontDestroyOnLoad' configuration.\n" +
                "You can don't use this.\n'CustomPersistentScene' Use, Uncheck here.", 
                MessageType.Info);
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_isAutoUpdateProp);
            EditorGUILayout.HelpBox(
                "AutoUpdate controls whether the Addressable system automatically updates the cached data after changes.", 
                MessageType.Info);
            EditorGUILayout.Space();
            DrawTitleImage();
            
            GUIStyle authorStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                normal =
                {
                    textColor = Color.gray
                }
            };
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Author: HuiSung, Song (ActionFit Inc)", authorStyle);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTitleLabel()
        {
            EditorGUILayout.Space();

            // Create a bold and italic style with increased font size
            GUIStyle boldItalicStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = Mathf.CeilToInt(EditorStyles.boldLabel.fontSize * 2.5f), // 2.5 times larger
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.MiddleCenter
            };

            // Create a horizontal group to draw the colored text
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Draw "A" in blue
            var blueTextStyle = new GUIStyle(boldItalicStyle);
            blueTextStyle.normal.textColor = Color.yellow;
            GUILayout.Label("A", blueTextStyle);
            
            // Draw " ddressable"
            GUILayout.Label("ddressable", boldItalicStyle);

            // Draw "S" in red
            var redTextStyle = new GUIStyle(boldItalicStyle);
            redTextStyle.normal.textColor = Color.red;
            GUILayout.Label(" S", redTextStyle);

            // Draw " ystem"
            GUILayout.Label("ystem", boldItalicStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void DrawTitleImage()
        {
            if (!_titleImage)
            {
                EditorGUILayout.HelpBox("Title image not found. please check the path.", MessageType.Warning);
                return;
            }

            // Begin a horizontal group and center the content within it
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Push content to the center
            GUILayout.Label(_titleImage, GUILayout.Width(_titleImage.width / 4f), GUILayout.Height(_titleImage.height / 4f));
            GUILayout.FlexibleSpace(); // Push content to the center
            GUILayout.EndHorizontal();
        }
    }
}