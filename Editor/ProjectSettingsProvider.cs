using System;
using System.Collections.Generic;
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

        public ProjectSettingsProvider(Type type) : base(GetSettingsPath(type), SettingsScope.Project)
        {
            if (!type.IsSubclassOf(typeof(ScriptableObject)))
            {
                throw new ProjectSettingsException($"Class must derive from ScriptableObject: '{type.Name}'");
            }

            _settingsType = type;
            _attribute = ProjectSettings.GetAttribute(type);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _object = ProjectSettings.LoadOrCreate(_settingsType);
            keywords = AssetSettingsProvider.GetSearchKeywordsFromSerializedObject(new SerializedObject(_object));
            
            _objectEditor = UnityEditor.Editor.CreateEditor(_object);
            if (_objectEditor.GetType().FullName.StartsWith("UnityEditor"))
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

        public static string GetSettingsPath(Type type)
        {
            return "Project/" + type.Name;
        }

        [SettingsProviderGroup]
        private static SettingsProvider[] GetProviders()
        {
            var providers = new List<SettingsProvider>();
            foreach (Type type in TypeCache.GetTypesWithAttribute<ProjectSettingsAttribute>())
            {
                try
                {
                    providers.Add(new ProjectSettingsProvider(type));
                }
                catch (ProjectSettingsException exception)
                {
                    providers.Add(new ErrorProjectSettingsProvider(type, exception));
                }
            }
            return providers.ToArray();
        }
    }

    public class ErrorProjectSettingsProvider : SettingsProvider
    {
        private Exception Exception;

        public ErrorProjectSettingsProvider(Type type, Exception exception) : base(ProjectSettingsProvider.GetSettingsPath(type), SettingsScope.Project)
        {
            Exception = exception;
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.HelpBox(Exception.Message, MessageType.Error);
        }
    }
}
