using UnityEngine;
using UnityEngine.UI;

namespace FirAnimations
{
    [RequireComponent(typeof(Image))]
    public class FirShaderFieldAnimation : FirAnimation
    {
        public string FieldName;
        public float Start;
        public float End;
        private float delta => End - Start;

        private Material _material;
        private Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = GetComponent<Image>().material;
                }
                return _material;
            }
        }
        
        private void Reset()
        {
            Material.SetFloat(FieldName, Start);
        }
        
        private void OnValidate()
        {
            if(EditorEnable)
                MoveByDelta();
        }
        protected override void MoveByDelta()
        {
            float curveValue = Curve.Evaluate(Time*_endTime);
            Material.SetFloat(FieldName, Start + (delta * curveValue));
        }
    }
}