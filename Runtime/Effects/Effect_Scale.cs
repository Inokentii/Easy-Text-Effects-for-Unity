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
        [Tooltip("Enable per-axis easing curves for squash/stretch effects.")]
        public bool useAxisEasing;

        [Tooltip("Override easing curve for X axis (defaults to main easing curve)."), SerializeField]
        private AnimationCurve axisEasingX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("Override easing curve for Y axis (defaults to main easing curve)."), SerializeField]
        private AnimationCurve axisEasingY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("Override easing curve for Z axis (defaults to main easing curve)."), SerializeField]
        private AnimationCurve axisEasingZ = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public override void StartEffect(TextEffectEntry entry)
        {
            base.StartEffect(entry);
            ConfigureAxisWrap(axisEasingX);
            ConfigureAxisWrap(axisEasingY);
            ConfigureAxisWrap(axisEasingZ);
        }

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
            float normalizedTime = GetNormalizedTime(charIndex);
            float tx = EvaluateAxisProgress(axisEasingX, normalizedTime);
            float ty = EvaluateAxisProgress(axisEasingY, normalizedTime);
            float tz = EvaluateAxisProgress(axisEasingZ, normalizedTime);

            float sx = Mathf.Lerp(startScale, endScale, tx);
            float sy = Mathf.Lerp(startScale, endScale, ty);
            float sz = Mathf.Lerp(startScale, endScale, tz);

            return new Vector3(sx, sy, sz);
        }

        private float EvaluateAxisProgress(AnimationCurve axisCurve, float normalizedTime)
        {
            var curve = axisCurve ?? easingCurve;
            float value = curve.Evaluate(normalizedTime);
            if (clampBetween0And1)
                value = Mathf.Clamp01(value);
            return value;
        }

        private float GetNormalizedTime(int charIndex)
        {
            if (!started)
                return 0f;

            float time = GetTimeForChar(charIndex);
            float duration = Mathf.Max(0.00001f, durationPerChar);
            float rawT = time / duration;

            if (animationType == AnimationType.OneTime || animationType == AnimationType.LoopFixedDuration)
                rawT = Mathf.Clamp01(rawT);

            return rawT;
        }

        private void ConfigureAxisWrap(AnimationCurve curve)
        {
            if (curve == null)
                return;

            switch (animationType)
            {
                case AnimationType.OneTime:
                case AnimationType.LoopFixedDuration:
                    curve.preWrapMode = WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Clamp;
                    break;
                case AnimationType.Loop:
                    curve.preWrapMode = noDelayForChars ? WrapMode.Loop : WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Loop;
                    break;
                case AnimationType.PingPong:
                    curve.preWrapMode = noDelayForChars ? WrapMode.PingPong : WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.PingPong;
                    break;
            }
        }
    }
}
