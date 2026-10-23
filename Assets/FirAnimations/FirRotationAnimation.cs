using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(RectTransform))]
    public class FirRotationAnimation : FirAnimation
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
            StartZoom = RectTransform.rotation.eulerAngles;
            EndZoom = StartZoom;
        }
        private void OnValidate()
        {
            if(EditorEnable)
                MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            RectTransform.rotation = Quaternion.Euler(StartZoom + (delta * curveValue));
        }
    }
}