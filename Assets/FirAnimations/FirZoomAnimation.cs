using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(RectTransform))]
    public class FirZoomAnimation : FirAnimation
    {
        public Vector3 StartZoom;
        public Vector3 EndZoom;

        private Vector3 delta => EndZoom - StartZoom;

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
            StartZoom = RectTransform.transform.localScale;
            EndZoom = StartZoom;
        }
        private void OnValidate()
        {
            MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            RectTransform.localScale = StartZoom + (delta * curveValue);
        }
    }
}