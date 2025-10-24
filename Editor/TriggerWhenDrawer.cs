#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EasyTextEffects.Editor
{
    [CustomPropertyDrawer(typeof(TextEffectEntry.TriggerWhen))]
    public class TriggerWhenDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] Options =
        {
            new GUIContent("On Start"),
            new GUIContent("Manual")
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int awakeIndex = (int)TextEffectEntry.TriggerWhen.OnAwake;
            int current = property.enumValueIndex;

            var effectProp = property.FindSiblingProperty("effect");
            bool isCurveEffect = effectProp != null && effectProp.objectReferenceValue is EasyTextEffects.Effects.Effect_Curve;
            if (isCurveEffect)
            {
                property.enumValueIndex = awakeIndex;
                current = awakeIndex;
            }
            else if (current == awakeIndex)
            {
                property.enumValueIndex = (int)TextEffectEntry.TriggerWhen.OnStart;
                current = property.enumValueIndex;
            }

            if (current == awakeIndex)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.LabelField(position, label, new GUIContent("On Awake (Layout)"));
                }
            }
            else
            {
                int selected = current == (int)TextEffectEntry.TriggerWhen.Manual ? 1 : 0;
                int newSelected = EditorGUI.Popup(position, label, selected, Options);
                property.enumValueIndex = newSelected == 1
                    ? (int)TextEffectEntry.TriggerWhen.Manual
                    : (int)TextEffectEntry.TriggerWhen.OnStart;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
