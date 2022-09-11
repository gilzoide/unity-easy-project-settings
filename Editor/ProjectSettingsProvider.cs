using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public class ProjectSettingsProvider : SettingsProvider
    {
        private Type _settingsType;
        private ProjectSettingsAttribute _attribute;
        private ScriptableObject _object;
        private UnityEditor.Editor _objectEditor;

        public ProjectSettingsProvider(Type type) : base(ProjectSettingsProviderGroup.GetSettingsPath(type), SettingsScope.Project)
        {
            if (!type.IsSubclassOf(typeof(ScriptableObject)))
            {
                throw new ProjectSettingsException($"Class must derive from ScriptableObject: '{type.Name}'");
            }

            _settingsType = type;
            _attribute = ProjectSettings.GetAttribute(type);
            if (string.IsNullOrWhiteSpace(_attribute.AssetPath))
            {
                throw new ProjectSettingsException($"ProjectSettingsAttribute cannot have an empty AssetPath: '{type.Name}'");
            }
            if (!string.IsNullOrWhiteSpace(_attribute.Label))
            {
                label = _attribute.Label;
            }
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _object = ProjectSettings.LoadOrCreate(_settingsType);
            keywords = AssetSettingsProvider.GetSearchKeywordsFromSerializedObject(new SerializedObject(_object));
            
            _objectEditor = UnityEditor.Editor.CreateEditor(_object);
            if (_objectEditor.GetType().FullName.StartsWith("UnityEditor."))
            {
                Object.DestroyImmediate(_objectEditor);
                _objectEditor = UnityEditor.Editor.CreateEditor(_object, typeof(SkipScriptEditor));
            }
        }

        public override void OnDeactivate()
        {
            ProjectSettings.Save(_object);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();
            _objectEditor.OnInspectorGUI();
        }

        public override void OnTitleBarGUI()
        {
            if (_attribute.IsRelativeToAssets && _object != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(_object, _settingsType, false);
                }
            }
        }
    }
}
