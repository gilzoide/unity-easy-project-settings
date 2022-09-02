using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gilzoide.EasyProjectSettings
{
    public static class ProjectSettings
    {
        public static T Load<T>() where T : ScriptableObject
        {
            return (T) Load(typeof(T));
        }
        public static ScriptableObject Load(Type type)
        {
            TryLoad(type, out ScriptableObject settingsObject);
            return settingsObject;
        }

        public static T LoadOrCreate<T>() where T : ScriptableObject
        {
            return (T) LoadOrCreate(typeof(T));
        }
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

        public static bool TryLoad<T>(out T settings) where T : ScriptableObject
        {
            bool result = TryLoad(typeof(T), out ScriptableObject settingsObject);
            settings = result ? (T) settingsObject : default;
            return result;
        }
        public static bool TryLoad(Type type, out ScriptableObject settings)
        {
            ProjectSettingsAttribute attribute = GetAttribute(type);
#if UNITY_EDITOR
            settings = (ScriptableObject) (attribute.IsRelativeToAssets
                ? AssetDatabase.LoadAssetAtPath(attribute.AssetPath, type)
                : UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(attribute.AssetPath).FirstOrDefault());
            return settings != null;
#else
            settings = Resources.Load<ScriptableObject>(attribute.ResourcesPath);
            return settings != null;
#endif
        }

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
            if (attribute.IsRelativeToAssets)
            {
                if (!AssetDatabase.Contains(obj))
                {
                    AssetDatabase.CreateAsset(obj, attribute.AssetPath);
                }
                AssetDatabase.SaveAssetIfDirty(obj);
            }
            else
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { obj }, attribute.AssetPath, true);
            }
        }
#endif
    }
}