using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeGraph.Editor
{
    public class CodeGraphEditorWindow : EditorWindow
    {
        [SerializeField] private CodeGraphAsset _currentGraph;
        [SerializeField] private CodeGraphView _currentView;
        [SerializeField] private SerializedObject _serializedObject;

        public CodeGraphAsset currentGraph => _currentGraph;

        public static void Open(CodeGraphAsset target)
        {
            CodeGraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<CodeGraphEditorWindow>();
            foreach(var w in windows)
            {
                if(w.currentGraph == target)
                {
                    w.Focus();
                    return;
                }
            }
            CodeGraphEditorWindow window = CreateWindow<CodeGraphEditorWindow>(typeof(CodeGraphEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null,typeof(CodeGraphAsset)).image);
            window.Load(target);
        }

        private void Load(CodeGraphAsset target)
        {
            _currentGraph = target;
        }
        
    }
}
