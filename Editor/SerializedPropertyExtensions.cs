#if UNITY_EDITOR
using UnityEditor;

namespace EasyTextEffects.Editor
{
    internal static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindSiblingProperty(this SerializedProperty property, string relativeName)
        {
            if (property == null || property.serializedObject == null)
                return null;

            var path = property.propertyPath;
            var lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
                return property.serializedObject.FindProperty(relativeName);

            var prefix = path.Substring(0, lastDot + 1);
            return property.serializedObject.FindProperty(prefix + relativeName);
        }
    }
}
#endif
