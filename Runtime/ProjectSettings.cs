using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gilzoide.EasyProjectSettings
{
    public static class ProjectSettings
    {
        /// <summary>
        /// Load the project settings of type <paramref name="type"/>.
        /// <para>
        /// Assets will not be loaded if not present in the path specified by the type's <see cref="ProjectSettingsAttribute.AssetPath"/>.
        /// They will also fail to load on built players if <see cref="ProjectSettingsAttribute.AssetPath"/> is not relative to a Resources folder.
        /// </para>
        /// </summary>
        /// <returns>
        /// The loaded project settings or <see langword="null"/> if loading failed.
        /// </returns>
        /// <param name="type"><see cref="ScriptableObject"/> subclass. Must have a <see cref="ProjectSettingsAttribute"/></param>
        /// <exception cref="ProjectSettingsException">Thrown when <paramref name="type"/> does not have a <see cref="ProjectSettingsAttribute"/></exception>
        public static ScriptableObject Load(Type type)
        {
            ProjectSettingsAttribute attribute = GetAttribute(type);
#if UNITY_EDITOR
            return (ScriptableObject) (attribute.IsRelativeToAssets
                ? AssetDatabase.LoadAssetAtPath(attribute.AssetPath, type)
                : UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(attribute.AssetPath).FirstOrDefault());
#else
            return Resources.Load<ScriptableObject>(attribute.ResourcesPath);
#endif
        }
        /// <summary>Typed version of <see cref="Load"/></summary>
        public static T Load<T>() where T : ScriptableObject
        {
            return (T) Load(typeof(T));
        }

        /// <summary>
        /// Try loading the project settings of type <paramref name="type"/> into <paramref name="settings"/>.
        /// <para>
        /// Assets will not be loaded if not present in the path specified by the type's <see cref="ProjectSettingsAttribute.AssetPath"/>.
        /// They will also fail to load on built players if <see cref="ProjectSettingsAttribute.AssetPath"/> is not relative to a Resources folder.
        /// </para>
        /// </summary>
        /// <returns>
        /// If the asset was succesfully loaded, <paramref name="settings"/> will contain the loaded asset and <see langword="true"/> will be returned.
        /// On failure, <paramref name="settings"/> will be <see langword="null"/> and <see langword="false"/> will be returned.
        /// </returns>
        /// <param name="type"><see cref="ScriptableObject"/> subclass. Must have a <see cref="ProjectSettingsAttribute"/></param>
        /// <exception cref="ProjectSettingsException">Thrown when <paramref name="type"/> does not have a <see cref="ProjectSettingsAttribute"/></exception>
        public static bool TryLoad(Type type, out ScriptableObject settings)
        {
            settings = Load(type);
            return settings != null;
        }
        /// <summary>Typed version of <see cref="TryLoad"/></summary>
        public static bool TryLoad<T>(out T settings) where T : ScriptableObject
        {
            settings = Load<T>();
            return settings != null;
        }

        /// <summary>
        /// Load the project settings of type <paramref name="type"/> or create a new instance if asset could not be loaded.
        /// <para>
        /// When running in the editor, new assets are created in disk if loading failed.
        /// </para>
        /// </summary>
        /// <returns>
        /// The loaded or created project settings.
        /// See <see cref="Load"/> for details on asset loading.
        /// </returns>
        /// <param name="type"><see cref="ScriptableObject"/> subclass. Must have a <see cref="ProjectSettingsAttribute"/></param>
        /// <exception cref="ProjectSettingsException">Thrown when <paramref name="type"/> does not have a <see cref="ProjectSettingsAttribute"/></exception>
        public static ScriptableObject LoadOrCreate(Type type)
        {
            if (!TryLoad(type, out ScriptableObject settingsObject))
            {
                settingsObject = ScriptableObject.CreateInstance(type);
#if UNITY_EDITOR
                Save(settingsObject);
#endif
            }
            return settingsObject;
        }
        /// <summary>Typed version of <see cref="LoadOrCreate"/></summary>
        public static T LoadOrCreate<T>() where T : ScriptableObject
        {
            return (T) LoadOrCreate(typeof(T));
        }

        /// <summary>Get the <see cref="ProjectSettingsAttribute"/> from <paramref name="type"/></summary>
        /// <exception cref="ProjectSettingsException">Thrown when <paramref name="type"/> does not have a <see cref="ProjectSettingsAttribute"/></exception>
        public static ProjectSettingsAttribute GetAttribute(Type type)
        {
            ProjectSettingsAttribute attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();
            if (attribute == null)
            {
                throw new ProjectSettingsException($"Type {type.Name} doesn't have a ProjectSettingsAttribute");
            }
            return attribute;
        }

#if UNITY_EDITOR
        public static void Save(Object obj)
        {
            if (obj == null)
            {
                return;
            }

            ProjectSettingsAttribute attribute = GetAttribute(obj.GetType());
            string directoryPath = Path.GetDirectoryName(attribute.AssetPath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            if (attribute.IsRelativeToAssets)
            {
                if (!AssetDatabase.Contains(obj))
                {
                    AssetDatabase.CreateAsset(obj, attribute.AssetPath);
                }
#if UNITY_2020_3_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(obj);
#endif
            }
            else
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { obj }, attribute.AssetPath, true);
            }
        }
#endif
    }
}
