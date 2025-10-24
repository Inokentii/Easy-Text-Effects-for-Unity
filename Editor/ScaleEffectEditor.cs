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
        private SerializedProperty clampProp;
        private SerializedProperty useAxisEasingProp;
        private SerializedProperty axisEasingXProp;
        private SerializedProperty axisEasingYProp;
        private SerializedProperty axisEasingZProp;
        private bool curveSectionDrawn;

        private void OnEnable()
        {
            easingCurveProp = serializedObject.FindProperty("easingCurve");
            clampProp = serializedObject.FindProperty("clampBetween0And1");
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
            curveSectionDrawn = false;
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

                if (iterator.propertyPath == "reverseCharOrder")
                {
                    EditorGUILayout.PropertyField(iterator, true);
                    DrawCurveSection();
                    continue;
                }

                if (iterator.propertyPath == useAxisEasingProp.propertyPath)
                {
                    // handled in curve section
                    continue;
                }

                if (iterator.propertyPath == easingCurveProp?.propertyPath ||
                    iterator.propertyPath == clampProp?.propertyPath ||
                    iterator.propertyPath == axisEasingXProp?.propertyPath ||
                    iterator.propertyPath == axisEasingYProp?.propertyPath ||
                    iterator.propertyPath == axisEasingZProp?.propertyPath)
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
            }

            if (!curveSectionDrawn)
                DrawCurveSection();
        }

        private void DrawCurveSection()
        {
            if (curveSectionDrawn)
                return;
            curveSectionDrawn = true;

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Curve", EditorStyles.boldLabel);

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

            EditorGUILayout.PropertyField(clampProp, new GUIContent("Clamp Between 0 And 1"));
            EditorGUILayout.Space(6f);
        }
    }
}
#endif
