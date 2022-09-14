using System;
using System.IO;

namespace Gilzoide.EasyProjectSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectSettingsAttribute : Attribute
    {
        public const string ResourcesDirectoryIdentifier = "/Resources/";

        public string AssetPath
        {
            get => _assetPath;
            set => _assetPath = Path.ChangeExtension(value, "asset");
        }
        public string SettingsPath { get; set; }
        public string Label { get; set; }

        private string _assetPath;

        public bool IsRelativeToAssets => AssetPath.StartsWith("Assets/");
        public bool IsRelativeToResources => AssetPath.IndexOf(ResourcesDirectoryIdentifier) >= 0;
        public string ResourcesPath
        {
            get
            {
                int resourcesIndex = AssetPath.IndexOf(ResourcesDirectoryIdentifier);
                return resourcesIndex >= 0
                    ? Path.ChangeExtension(AssetPath.Substring(resourcesIndex + ResourcesDirectoryIdentifier.Length), null)
                    : null;
            }
        }

        public ProjectSettingsAttribute(string assetPath)
        {
            AssetPath = assetPath;
        }
    }
}
