#if UNITY_EDITOR
using EasyTextEffects.Effects;
using UnityEditor;
using UnityEngine;

namespace EasyTextEffects.Editor
{
    [CustomEditor(typeof(Effect_Scale))]
    public class ScaleEffectEditor : UnityEditor.Editor
    {
        private SerializedProperty easingCurveProp;
        private SerializedProperty useAxisEasingProp;
        private SerializedProperty axisEasingXProp;
        private SerializedProperty axisEasingYProp;
        private SerializedProperty axisEasingZProp;

        private void OnEnable()
        {
            easingCurveProp = serializedObject.FindProperty("easingCurve");
            useAxisEasingProp = serializedObject.FindProperty("useAxisEasing");
            axisEasingXProp = serializedObject.FindProperty("axisEasingX");
            axisEasingYProp = serializedObject.FindProperty("axisEasingY");
            axisEasingZProp = serializedObject.FindProperty("axisEasingZ");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            DrawProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (target is Effect_Scale effect)
                    effect.HandleValueChanged();
            }
            else
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawProperties()
        {
            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(iterator, true);
                    continue;
                }

                if (iterator.propertyPath == useAxisEasingProp.propertyPath)
                {
                    EditorGUILayout.PropertyField(useAxisEasingProp, new GUIContent("Use Per Axis Easing"));
                    if (useAxisEasingProp.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(axisEasingXProp, new GUIContent("Easing X"));
                        EditorGUILayout.PropertyField(axisEasingYProp, new GUIContent("Easing Y"));
                        EditorGUILayout.PropertyField(axisEasingZProp, new GUIContent("Easing Z"));
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(easingCurveProp, new GUIContent("Easing Curve"));
                    }
                    continue;
                }

                if (iterator.propertyPath == easingCurveProp?.propertyPath ||
                    iterator.propertyPath == axisEasingXProp?.propertyPath ||
                    iterator.propertyPath == axisEasingYProp?.propertyPath ||
                    iterator.propertyPath == axisEasingZProp?.propertyPath)
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
}
#endif
