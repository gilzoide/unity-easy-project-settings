using System;
using System.Linq;
using UnityEditor;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public class ErrorProjectSettingsProvider : SettingsProvider
    {
        private MonoScript _script;
        private Exception _exception;

        public ErrorProjectSettingsProvider(Type type, Exception exception)
            : base(ProjectSettingsProviderGroup.GetSettingsPath(type), ProjectSettingsProviderGroup.GetSettingsScope(type))
        {
            _exception = exception;
            _script = GetScriptForType(type);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.HelpBox(_exception.Message, MessageType.Error);
            if (_script)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false);
                }
            }
        }

        private static MonoScript GetScriptForType(Type type)
        {
            return AssetDatabase.FindAssets($"t:MonoScript {type.Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .FirstOrDefault();
        }
    }
}