using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NG.Windows {

    public class DungeonEditorWindow : EditorWindow {

        [MenuItem("Custom Tools/Dungeon Editor/DungeonEditorWindow")]
        public static void Open() {
            DungeonEditorWindow wnd = GetWindow<DungeonEditorWindow>();
            wnd.titleContent = new GUIContent("Dungeon Editor");
        }


        
        public void CreateGUI()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView() {
            DungeonGraphView graphView = new DungeonGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void AddStyles() {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DungeonSystem/NGVariables.uss");

            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}
