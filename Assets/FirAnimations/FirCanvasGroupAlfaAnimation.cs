using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FirCanvasGroupAlfaAnimation : FirAnimation
    {
        public float StartAlfa = 0;
        public float EndAlfa = 1;
        private float delta => EndAlfa - StartAlfa;

        private CanvasGroup _group;
        private CanvasGroup group
        {
            get
            {
                if (_group == null)
                {
                    _group = GetComponent<CanvasGroup>();
                }

                return _group;
            }
        }
        
        private void Reset()
        {
            StartAlfa = group.alpha;
            EndAlfa = StartAlfa;
        }
        
        private void OnValidate()
        {
            if(EditorEnable)
                MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            group.alpha = StartAlfa + (delta * curveValue);
        }
    }
}