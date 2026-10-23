using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FirAnimations
{
    public abstract class FirAnimation : MonoBehaviour
    {
        public bool EditorEnable = true;
        [Range(0, 1)]
        protected float Time;
        public bool Loop;
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0,0,1,1);
        protected float _endTime => Curve.keys[Curve.length-1].time;
        public Action OnComplete;
        
        public virtual void Initialize()
        {
            Stop();
            ToStartPoint();
        }
        
        public virtual void Play()
        { 
            ToStartPoint();
            enabled = true;
        }

        public virtual void Stop()
        {
            ToEndPoint();
            enabled = false;
        }

        [ContextMenu("ToStartPoint")]
        public virtual void ToStartPoint()
        {
            Time = 0;
            MoveByDelta();
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        [ContextMenu("ToEndPoint")]
        public virtual void ToEndPoint()
        {
            Time = 1;
            MoveByDelta();
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        public void SetTime(float time)
        {
            Time = time / _endTime;
        }
        
        public void Update()
        {
            if (Time >= 1)
            {
                if (Loop)
                {
                    Time %= 1;
                }
                else
                {
                    Time = 1;
                    enabled = false;
                    MoveByDelta();
                    OnComplete?.Invoke();
                    return;
                }
            }
            
            Time += UnityEngine.Time.unscaledDeltaTime/_endTime;
            MoveByDelta();
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        protected abstract void MoveByDelta();
    }
}