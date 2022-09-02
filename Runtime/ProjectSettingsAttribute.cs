using System;
using System.IO;

namespace Gilzoide.EasyProjectSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectSettingsAttribute : Attribute
    {
        public const string ResourcesDirectoryIdentifier = "/Resources/";

        public string FilePath;
        public string SettingsPath;
        public string Label;

        public bool IsRelativeToAssets => FilePath.StartsWith("Assets/");
        public bool IsRelativeToResources => FilePath.IndexOf(ResourcesDirectoryIdentifier) >= 0;

        public ProjectSettingsAttribute(string filePath, string settingsPath = null, string label = null)
        {
            if (Path.IsPathRooted(filePath))
            {
                throw new ProjectSettingsException($"FilePath must not be a rooted path: '{filePath}'");
            }
            FilePath = Path.ChangeExtension(filePath, "asset");
            SettingsPath = settingsPath;
            Label = label;
        }

        public string GetResourcesPath()
        {
            int resourcesIndex = FilePath.IndexOf(ResourcesDirectoryIdentifier);
            if (resourcesIndex < 0)
            {
                throw new ProjectSettingsException($"FilePath is not a Resources directory: '{FilePath}'");
            }
            return Path.ChangeExtension(FilePath.Substring(resourcesIndex + ResourcesDirectoryIdentifier.Length), null);
        }
    }
}
