using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(RectTransform))]
    public class FirSizeAnimation : FirAnimation
    {
        public Vector2 StartSize;
        public Vector2 EndSize;
        private Vector2 delta => EndSize - StartSize;

        private RectTransform rectTransform;
        private RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }
        
        private void Reset()
        {
            StartSize = RectTransform.sizeDelta;
            EndSize = StartSize;
        }
        
        private void OnValidate()
        {
            if(EditorEnable)
                MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            RectTransform.sizeDelta = StartSize + (delta * curveValue);
        }
    }
}