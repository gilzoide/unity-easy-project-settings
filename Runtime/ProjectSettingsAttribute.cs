using System;
using System.IO;

namespace Gilzoide.EasyProjectSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectSettingsAttribute : Attribute
    {
        public const string ResourcesDirectoryIdentifier = "/Resources/";

        public string AssetPath;
        public string SettingsPath;
        public string Label;

        public bool IsRelativeToAssets => AssetPath.StartsWith("Assets/");
        public bool IsRelativeToResources => AssetPath.IndexOf(ResourcesDirectoryIdentifier) >= 0;
        public string ResourcesPath
        {
            get
            {
                int resourcesIndex = AssetPath.IndexOf(ResourcesDirectoryIdentifier);
                if (resourcesIndex < 0)
                {
                    throw new ProjectSettingsException($"{nameof(AssetPath)} is not a Resources directory: '{AssetPath}'");
                }
                return Path.ChangeExtension(AssetPath.Substring(resourcesIndex + ResourcesDirectoryIdentifier.Length), null);
            }
        }

        public ProjectSettingsAttribute(string assetPath, string settingsPath = null, string label = null)
        {
            if (Path.IsPathRooted(assetPath))
            {
                throw new ProjectSettingsException($"{nameof(AssetPath)} must not be a rooted path: '{assetPath}'");
            }
            AssetPath = Path.ChangeExtension(assetPath, "asset");
            SettingsPath = settingsPath;
            Label = label;
        }
    }
}
