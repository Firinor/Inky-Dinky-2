using UnityEngine;

namespace FirAnimations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FirColorSpriteAnimation : FirAnimation
    {
        public Color StartPosition = new Color(1,1,1,0);
        public Color EndPosition = Color.white;
        private Color delta => EndPosition - StartPosition;

        private SpriteRenderer _sprite;
        private SpriteRenderer sprite
        {
            get
            {
                if (_sprite == null)
                {
                    _sprite = GetComponent<SpriteRenderer>();
                }

                return _sprite;
            }
        }
        
        private void Reset()
        {
            StartPosition = sprite.color;
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
            sprite.color = Color.LerpUnclamped(StartPosition, EndPosition, curveValue);
        }
    }
}