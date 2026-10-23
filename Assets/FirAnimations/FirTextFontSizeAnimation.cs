using TMPro;
using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FirTextFontSizeAnimation : FirAnimation
    {
        public float StartPosition;
        public float EndPosition;
        private float delta => EndPosition - StartPosition;
        
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
            StartPosition = text.fontSize;
            EndPosition = StartPosition;
        }
        private void OnValidate()
        {
            MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            text.fontSize = StartPosition + (delta * curveValue);
        }
    }
}