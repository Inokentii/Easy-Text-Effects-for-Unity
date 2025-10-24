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
        [Header("Non-Uniform")]
        [Tooltip("Enable per-axis animation curves for squash and stretch.")]
        public bool useAxisCurves;

        [Tooltip("X-axis scale curve (evaluated after easing).")]
        public AnimationCurve scaleXCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [Tooltip("Y-axis scale curve (evaluated after easing).")]
        public AnimationCurve scaleYCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [Tooltip("Z-axis scale curve (evaluated after easing).")]
        public AnimationCurve scaleZCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public override void ApplyEffect(TMP_TextInfo _textInfo, int _charIndex, int _startVertex = 0, int _endVertex = 3)
        {
            if (!CheckCanApplyEffect(_charIndex)) return;

            TMP_CharacterInfo charInfo = _textInfo.characterInfo[_charIndex];
            var materialIndex = charInfo.materialReferenceIndex;
            var verts = _textInfo.meshInfo[materialIndex].vertices;
            Vector3 center = CharCenter(charInfo, verts);

            float uniformScale = Interpolate(startScale, endScale, _charIndex);
            Vector3 finalScale;

            if (useAxisCurves)
            {
                float progress = GetProgress01(_charIndex);
                float eased = easingCurve.Evaluate(progress);
                if (clampBetween0And1)
                    eased = Mathf.Clamp01(eased);

                float sx = scaleXCurve != null ? scaleXCurve.Evaluate(eased) : 1f;
                float sy = scaleYCurve != null ? scaleYCurve.Evaluate(eased) : 1f;
                float sz = scaleZCurve != null ? scaleZCurve.Evaluate(eased) : 1f;

                Vector3 axisScale = new Vector3(sx, sy, sz);
                Vector3 uniformVec = new Vector3(uniformScale, uniformScale, uniformScale);
                finalScale = Vector3.Scale(axisScale, uniformVec);
            }
            else
            {
                finalScale = new Vector3(uniformScale, uniformScale, uniformScale);
            }

            for (var v = _startVertex; v <= _endVertex; v++)
            {
                var vertexIndex = charInfo.vertexIndex + v;
                Vector3 fromCenter = verts[vertexIndex] - center;
                fromCenter = Vector3.Scale(fromCenter, finalScale);
                verts[vertexIndex] = center + fromCenter;
            }
        }
    }
}
