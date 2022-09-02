using System;
using UnityEditor;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public class ErrorProjectSettingsProvider : SettingsProvider
    {
        private Exception _exception;

        public ErrorProjectSettingsProvider(Type type, Exception exception) : base(ProjectSettingsProviderGroup.GetSettingsPath(type), SettingsScope.Project)
        {
            _exception = exception;
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.HelpBox(_exception.Message, MessageType.Error);
        }
    }
}