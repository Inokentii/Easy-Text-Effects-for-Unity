#if UNITY_EDITOR
using UnityEditor;

namespace EasyTextEffects.Editor
{
    [CustomEditor(typeof(EasyTextEffects.Effects.Effect_Curve))]
    public class EffectCurveEditor : UnityEditor.Editor
    {
        private SerializedProperty effectTagProp_;
        private SerializedProperty curveProp_;
        private SerializedProperty amplitudeProp_;
        private SerializedProperty applyRotationProp_;

        private void OnEnable()
        {
            effectTagProp_ = serializedObject.FindProperty("effectTag");
            curveProp_ = serializedObject.FindProperty("curve");
            amplitudeProp_ = serializedObject.FindProperty("amplitude");
            applyRotationProp_ = serializedObject.FindProperty("applyRotation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (effectTagProp_ != null)
                EditorGUILayout.PropertyField(effectTagProp_);

            if (curveProp_ != null)
                EditorGUILayout.PropertyField(curveProp_);

            if (amplitudeProp_ != null)
                EditorGUILayout.PropertyField(amplitudeProp_);

            if (applyRotationProp_ != null)
                EditorGUILayout.PropertyField(applyRotationProp_);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
