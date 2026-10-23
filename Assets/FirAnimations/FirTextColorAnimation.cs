using TMPro;
using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FirTextColorAnimation : FirAnimation
    {
        public Color StartPosition = new Color(1,1,1,0);
        public Color EndPosition = Color.white;
        private Color delta => EndPosition - StartPosition;

        private TextMeshProUGUI _text;
        private TextMeshProUGUI text
        {
            get
            {
                if (_text == null)
                {
                    _text = GetComponent<TextMeshProUGUI>();
                }

                return _text;
            }
        }
        private void Reset()
        {
            StartPosition = text.color;
            EndPosition = StartPosition;
        }
        private void OnValidate()
        {
            MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            text.color = Color.LerpUnclamped(StartPosition, EndPosition, curveValue);
        }
    }
}