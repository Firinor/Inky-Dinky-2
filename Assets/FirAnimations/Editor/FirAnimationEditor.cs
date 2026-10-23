using UnityEditor;
using UnityEngine;

namespace FirAnimations
{
    [CustomEditor(typeof(FirAnimation), editorForChildClasses: true)]
    public class FirAnimationEditor : Editor
    {
        private bool isPlaying;

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                DrawDefaultInspector();
                return;
            }

            FirAnimation script = (FirAnimation)target;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<<"))
            {
                Off();
                script.ToStartPoint();
            }

            if (GUILayout.Button(isPlaying ? "ll" : ">"))
            {
                if (isPlaying)
                {
                    Off();
                }
                else if(script.EditorEnable)
                {
                    EditorApplication.update += script.Update;
                    isPlaying = true;
                    script.OnComplete += Off;
                    script.Initialize();
                    script.ToStartPoint();
                }
                else
                {
                    Off();
                }
            }

            if (GUILayout.Button(">>"))
            {
                Off();
                script.ToEndPoint();
            }

            EditorGUILayout.EndHorizontal();
            DrawDefaultInspector();
        }

        private void Off()
        {
            FirAnimation script = (FirAnimation)target;

            isPlaying = false;
            script.OnComplete -= Off;
            EditorApplication.update -= script.Update;
        }
    }
    
    [CustomEditor(typeof(FirRotate))]
    public class FirRotateEditor : Editor
    {
        private bool isPlaying;

        public override void OnInspectorGUI()
        {
            FirRotate script = (FirRotate)target;
            
            if (EditorApplication.isPlaying)
            {
                ActiveFields();
                return;
            }
            
            if (GUILayout.Button(isPlaying ? "ll" : ">"))
            {
                if (isPlaying)
                {
                    Off();
                }
                else if(script.EditorEnable)
                {
                    EditorApplication.update += script.Update;
                    isPlaying = true;
                    script.OnComplete += Off;
                    script.Initialize();
                    script.ToStartPoint();
                }
                else
                {
                    Off();
                }
            }
            ActiveFields();
        }

        private void ActiveFields()
        {
            FirRotate script = (FirRotate)target;
            
            script.EditorEnable = EditorGUILayout.Toggle(label: nameof(script.EditorEnable), script.EditorEnable);
            script.StartPosition = EditorGUILayout.FloatField(label: nameof(script.StartPosition), script.StartPosition);
            script.Speed = EditorGUILayout.FloatField(label: nameof(script.Speed), script.Speed);
        }

        private void Off()
        {
            FirRotate script = (FirRotate)target;

            isPlaying = false;
            script.OnComplete -= Off;
            EditorApplication.update -= script.Update;
        }
    }
}