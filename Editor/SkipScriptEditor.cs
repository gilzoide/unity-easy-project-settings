using UnityEditor;

namespace Gilzoide.EasyProjectSettings.Editor
{
    public class SkipScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            SerializedProperty iterator = serializedObject.GetIterator().Copy();
            if (iterator.NextVisible(true) && iterator.name != "m_Script")
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
            while (iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}