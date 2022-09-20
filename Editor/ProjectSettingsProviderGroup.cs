using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public static class ProjectSettingsProviderGroup
    {
        public static string GetSettingsPath(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettings.GetAttribute(type);
            if (!string.IsNullOrWhiteSpace(attribute?.SettingsPath))
            {
                return attribute.SettingsPath;
            }
            
            switch (attribute.SettingsScope)
            {
                case SettingsScope.User:
                    return "Preferences/" + type.Name;

                case SettingsScope.Project:
                    return "Project/" + type.Name;

                default:
                    return type.Name;
            }
        }

        public static SettingsScope GetSettingsScope(Type type)
        {
            return ProjectSettings.GetAttribute(type).SettingsScope;
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
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return providers.ToArray();
        }
    }
}