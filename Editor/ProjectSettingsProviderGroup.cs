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
            
            switch (attribute.SettingsType)
            {
                case SettingsType.ProjectSettings:
                    return "Project/" + type.Name;

                case SettingsType.UserSettings:
                    return "Preferences/" + type.Name;

                default:
                    return type.Name;
            }
        }

        public static SettingsScope GetSettingsScope(Type type)
        {
            switch (ProjectSettings.GetAttribute(type).SettingsType)
            {
                case SettingsType.UserSettings:
                    return SettingsScope.User;
                
                case SettingsType.ProjectSettings:
                default:
                    return SettingsScope.Project;
            }
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