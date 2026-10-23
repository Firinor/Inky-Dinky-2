using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(RectTransform))]
    public class FirPositionAnimation : FirAnimation
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        private Vector3 delta => EndPosition - StartPosition;

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
            StartPosition = RectTransform.anchoredPosition3D;
            EndPosition = StartPosition;
        }
        
        private void OnValidate()
        {
            if(EditorEnable)
                MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            RectTransform.anchoredPosition3D = StartPosition + (delta * curveValue);
        }
    }
}