using System;
using System.IO;

namespace Gilzoide.EasyProjectSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProjectSettingsAttribute : Attribute
    {
        public const string AssetsDirectoryIdentifier = "Assets/";
        public const string ResourcesDirectoryIdentifier = "/Resources/";

        /// <summary>
        /// Path where project settings asset will be loaded from/saved to.
        /// Since project settings must be ScriptableObjects, the setter forces a ".asset" extension.
        /// <para>
        /// If the given path is relative to a "Resources" folder, the asset will be loadable by
        /// code in built players by using <see cref="ProjectSettings.Load"/>
        /// </para>
        /// </summary>
        public string AssetPath
        {
            get => _assetPath;
            set => _assetPath = Path.ChangeExtension(value, "asset");
        }
        private string _assetPath;
        /// <summary>Determines whether settings appear in the Project Settings window or in the Preferences window</summary>
        public SettingsType SettingsType { get; set; } = SettingsType.ProjectSettings;
        /// <summary>
        /// Path used to place the SettingsProvider in the tree view of the Settings window.
        /// The path should be unique among all other settings paths and should use "/" as its separator.
        /// </summary>
        public string SettingsPath { get; set; }
        /// <summary>Display name of the SettingsProvider as it appears in the Settings window</summary>
        public string Label { get; set; }

        /// <summary>Whether <see cref="AssetPath"/> is relative to the "Assets" folder</summary>
        public bool IsRelativeToAssets => AssetPath.StartsWith(AssetsDirectoryIdentifier);
        /// <summary>Whether <see cref="AssetPath"/> is relative to a "Resources" folder</summary>
        public bool IsRelativeToResources => AssetPath.IndexOf(ResourcesDirectoryIdentifier) >= 0;
        /// <summary>
        /// If <see cref="AssetPath"/> is relative to a "Resources" folder, gets the relative path for loading the
        /// asset with <a href="https://docs.unity3d.com/ScriptReference/Resources.Load.html">Resources.Load</a>.
        /// Returns <see langword="null"/> otherwise.
        /// </summary>
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
