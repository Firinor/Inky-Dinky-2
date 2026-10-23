using UnityEngine;
using UnityEngine.UI;

namespace FirAnimations
{
    [RequireComponent(typeof(Image))]
    public class FirColorAnimation : FirAnimation
    {
        public Color StartPosition = new Color(1,1,1,0);
        public Color EndPosition = Color.white;
        private Color delta => EndPosition - StartPosition;

        private Image _image;
        private Image image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }

                return _image;
            }
        }
        
        private void Reset()
        {
            StartPosition = image.color;
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
            image.color = Color.LerpUnclamped(StartPosition, EndPosition, curveValue);
        }
    }
}
namespace FirAnimations
{
}