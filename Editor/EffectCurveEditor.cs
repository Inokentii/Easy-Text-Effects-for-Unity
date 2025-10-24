#if UNITY_EDITOR
using UnityEditor;

namespace EasyTextEffects.Editor
{
    [CustomEditor(typeof(EasyTextEffects.Effects.Effect_Curve))]
    public class EffectCurveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectTag"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("curve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("amplitude"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("applyRotation"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
