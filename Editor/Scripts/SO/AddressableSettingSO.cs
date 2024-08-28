
using UnityEditor;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class AddressableSettingSO : ScriptableObject
    {
        #region Fields

        [Header("Debug Settings")] 
        [SerializeField] private ExceptionHandleTypes _exceptionHandleType = ExceptionHandleTypes.Log;
        [SerializeField] private bool _isDebug;

        [Header("Object Config")] 
        [SerializeField] public bool UseDontDestroyOnLoad;

        [Header("Update Cached Settings")] 
        [SerializeField] public bool IsAutoUpdate = true;

        private bool _previousIsDebug;
        private bool _previousIsAutoUpdate;

        public ExceptionHandleTypes ExceptionHandleType => _exceptionHandleType;

        #endregion



        #region Enabled

        private void OnEnable()
        {
            _previousIsDebug = _isDebug;
            _previousIsAutoUpdate = IsAutoUpdate;
        }

        #endregion
        


        #region Validate

        private void OnValidate()
        {
            if (_isDebug != _previousIsDebug)
            {
                OnDebugEvents();
                _previousIsDebug = _isDebug;
            }

            if (IsAutoUpdate != _previousIsAutoUpdate)
            {
                OnUpdateStateChange();
                _previousIsAutoUpdate = IsAutoUpdate;
            }
            
            UpdateConvertSetting();
        }

        private void OnDebugEvents()
        {
            if (_isDebug)
            {
                AddressableDefineSymbols.EnableDebug();
            }
            else
            {
                AddressableDefineSymbols.DisableDebug();
            }
        }

        private void OnUpdateStateChange()
        {
            ForceRecompile();
        }

        private void ForceRecompile()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            AssetDatabase.Refresh();
        }

        #endregion
        
        
        
        #region Update Convert Setting

        private void UpdateConvertSetting()
        {
#if UNITY_EDITOR
            string convertAssetPath = "Assets/Resources/ScriptableObjects/Addressables/AddressableSystemSettingConvert.asset";
            var convertSetting = AssetDatabase.LoadAssetAtPath<AddressableSettingConvertSO>(convertAssetPath);

            if (convertSetting != null)
            {
                convertSetting.SetExceptionType(_exceptionHandleType);
                convertSetting.SetDontLoadConfig(UseDontDestroyOnLoad);

                EditorUtility.SetDirty(convertSetting);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError($"[Addressable System] Convert Setting not found at {convertAssetPath}. Please ensure the asset exists.");
            }
#endif
        }

        #endregion
    }
}