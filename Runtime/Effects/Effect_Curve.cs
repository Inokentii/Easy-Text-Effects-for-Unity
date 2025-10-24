using EasyTextEffects;
using TMPro;
using UnityEngine;

namespace EasyTextEffects.Effects
{
    [CreateAssetMenu(fileName = "Curved Layout", menuName = "Easy Text Effects/0. Curved Layout", order = 0)]
    public class Effect_Curve : TextEffectInstance
    {
        [Tooltip("0..1 X => vertical offset. Use smooth bell shape by default.")]
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.5f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, 0f, 0f)
        );

        [Tooltip("Vertical amplitude (pixels for UGUI; local units for 3D TMP).")]
        public float amplitude = 32f;

        [Tooltip("Rotate glyph quads to follow the curve tangent.")]
        public bool applyRotation = true;

        public override void StartEffect(TextEffectEntry entry)
        {
            if (entry != null)
                entry.triggerWhen = TextEffectEntry.TriggerWhen.OnAwake;
            base.StartEffect(entry);
        }

        public override void ApplyEffect(TMP_TextInfo textInfo, int charIndex, int startVertex = 0, int endVertex = 3)
        {
            if (charIndex < startCharIndex || charIndex >= startCharIndex + charLength)
                return;

            CacheSource(textInfo);

            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible)
                return;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            var source = cachedSourceVertices?[materialIndex];
            if (source == null)
                return;

            var destination = textInfo.meshInfo[materialIndex].vertices;

            Vector3 p0 = source[vertexIndex + 0];
            Vector3 p1 = source[vertexIndex + 1];
            Vector3 p2 = source[vertexIndex + 2];
            Vector3 p3 = source[vertexIndex + 3];

            Vector3 bottomL = p0.y <= p3.y ? p0 : p3;
            Vector3 bottomR = p0.y <= p3.y ? p3 : p0;
            Vector3 pivot = (bottomL + bottomR) * 0.5f;

            float charMinX = Mathf.Min(p0.x, Mathf.Min(p1.x, Mathf.Min(p2.x, p3.x)));
            float charMaxX = Mathf.Max(p0.x, Mathf.Max(p1.x, Mathf.Max(p2.x, p3.x)));
            float charCenterX = (charMinX + charMaxX) * 0.5f;

            float width = maxX - minX;
            float t = Mathf.InverseLerp(minX, maxX, charCenterX);
            float yOffset = curve.Evaluate(t) * amplitude;

            float angle = 0f;
            if (applyRotation)
            {
                float epsT = 1f / Mathf.Max(1024f, width);
                float t2 = Mathf.Clamp01(t + epsT);
                float dy = (curve.Evaluate(t2) - curve.Evaluate(t)) * amplitude;
                float dx = (t2 - t) * width;
                angle = Mathf.Atan2(dy, Mathf.Max(0.00001f, dx));
            }

            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            for (int k = 0; k < 4; k++)
            {
                Vector3 original = source[vertexIndex + k];
                Vector3 delta = original - pivot;

                float rotatedX = delta.x * cos - delta.y * sin;
                float rotatedY = delta.x * sin + delta.y * cos;

                Vector3 transformed = new Vector3(pivot.x + rotatedX, pivot.y + rotatedY + yOffset, original.z);
                destination[vertexIndex + k] = transformed;
            }
        }

        private Vector3[][] cachedSourceVertices;
        private float minX;
        private float maxX;
        private bool cacheValid;

        private void OnEnable()
        {
            cacheValid = false;
            HandleValueChanged();
        }

        private void OnDisable()
        {
            cacheValid = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            cacheValid = false;
            HandleValueChanged();
        }
#endif

        private void CacheSource(TMP_TextInfo textInfo)
        {
            if (cacheValid)
                return;

            int meshCount = textInfo.meshInfo.Length;
            if (cachedSourceVertices == null || cachedSourceVertices.Length != meshCount)
            {
                cachedSourceVertices = new Vector3[meshCount][];
            }

            minX = float.PositiveInfinity;
            maxX = float.NegativeInfinity;

            for (int m = 0; m < meshCount; m++)
            {
                var vertices = textInfo.meshInfo[m].vertices;
                if (vertices == null)
                {
                    cachedSourceVertices[m] = null;
                    continue;
                }

                var copy = cachedSourceVertices[m];
                if (copy == null || copy.Length != vertices.Length)
                {
                    copy = new Vector3[vertices.Length];
                    cachedSourceVertices[m] = copy;
                }

                for (int v = 0; v < vertices.Length; v++)
                {
                    Vector3 vertex = vertices[v];
                    copy[v] = vertex;
                    float x = vertex.x;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }
            }

            if (maxX <= minX)
            {
                maxX = minX + 0.0001f;
            }

            cacheValid = true;
        }
    }
}
