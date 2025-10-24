using TMPro;
using UnityEngine;

namespace EasyTextEffects.Effects
{
    [CreateAssetMenu(fileName = "Scale", menuName = "Easy Text Effects/4. Scale", order = 4)]
    public class Effect_Scale : TextEffectInstance
    {
        [Space(10)]
        [Header("Scale")]
        public float startScale = 0;
        public float endScale = 1;

        [Space(10)]
        [Header("Per-Axis Easing")]
        [Tooltip("Enable per-axis easing curves for squash/stretch effects.")]
        public bool useAxisEasing;

        [Tooltip("Override easing curve for X axis (defaults to main easing curve).")]
        public AnimationCurve axisEasingX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("Override easing curve for Y axis (defaults to main easing curve).")]
        public AnimationCurve axisEasingY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("Override easing curve for Z axis (defaults to main easing curve).")]
        public AnimationCurve axisEasingZ = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public override void ApplyEffect(TMP_TextInfo _textInfo, int _charIndex, int _startVertex = 0, int _endVertex = 3)
        {
            if (!CheckCanApplyEffect(_charIndex)) return;

            TMP_CharacterInfo charInfo = _textInfo.characterInfo[_charIndex];
            var materialIndex = charInfo.materialReferenceIndex;
            var verts = _textInfo.meshInfo[materialIndex].vertices;
            Vector3 center = CharCenter(charInfo, verts);

            Vector3 finalScale = useAxisEasing
                ? EvaluateAxisScale(_charIndex)
                : Vector3.one * Interpolate(startScale, endScale, _charIndex);

            for (var v = _startVertex; v <= _endVertex; v++)
            {
                var vertexIndex = charInfo.vertexIndex + v;
                Vector3 fromCenter = verts[vertexIndex] - center;
                fromCenter = Vector3.Scale(fromCenter, finalScale);
                verts[vertexIndex] = center + fromCenter;
            }
        }

        private Vector3 EvaluateAxisScale(int charIndex)
        {
            float duration = Mathf.Max(0.00001f, durationPerChar);
            float time = GetTimeForChar(charIndex);
            float normalizedTime = time / duration;

            float tx = axisEasingX != null ? axisEasingX.Evaluate(normalizedTime) : easingCurve.Evaluate(normalizedTime);
            float ty = axisEasingY != null ? axisEasingY.Evaluate(normalizedTime) : easingCurve.Evaluate(normalizedTime);
            float tz = axisEasingZ != null ? axisEasingZ.Evaluate(normalizedTime) : easingCurve.Evaluate(normalizedTime);

            if (clampBetween0And1)
            {
                tx = Mathf.Clamp01(tx);
                ty = Mathf.Clamp01(ty);
                tz = Mathf.Clamp01(tz);
            }

            float sx = Mathf.Lerp(startScale, endScale, tx);
            float sy = Mathf.Lerp(startScale, endScale, ty);
            float sz = Mathf.Lerp(startScale, endScale, tz);

            return new Vector3(sx, sy, sz);
        }
    }
}
