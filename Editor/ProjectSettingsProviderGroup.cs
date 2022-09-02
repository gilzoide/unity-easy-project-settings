using System;
using System.Collections.Generic;
using UnityEditor;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public static class ProjectSettingsProviderGroup
    {
        public static string GetSettingsPath(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettings.GetAttribute(type);
            return !string.IsNullOrWhiteSpace(attribute?.SettingsPath)
                ? attribute.SettingsPath
                : "Project/" + type.Name;
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
}