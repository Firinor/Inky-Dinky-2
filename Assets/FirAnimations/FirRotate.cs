using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FirAnimations
{
    [RequireComponent(typeof(RectTransform))]
    public class FirRotate : FirAnimation
    {
        public float StartPosition;
        public float Speed = 1;

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
            StartPosition = RectTransform.rotation.eulerAngles.z;
        }
        
        public override void ToStartPoint()
        {
            ToEndPoint();
        }

        [ContextMenu("ToStartPointRotation")]
        public override void ToEndPoint()
        {
            Time = 0;
            RectTransform.localRotation = Quaternion.Euler(0,0,StartPosition);
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        
        public new void Update()
        {
            Time  += UnityEngine.Time.unscaledDeltaTime;

            MoveByDelta();
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        
        protected override void MoveByDelta()
        {
            RectTransform.localRotation *= Quaternion.Euler(0,0,Speed * UnityEngine.Time.unscaledDeltaTime * 360);
        }
    }
}