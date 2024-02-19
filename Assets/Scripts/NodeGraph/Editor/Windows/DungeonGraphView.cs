using NG.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NG.Windows {

    public class DungeonGraphView : GraphView
    {
        public DungeonGraphView() {
            AddManipulators();
            AddGridBackground();
            AddStyles();
        }

        private NGNode CreateNode(Vector2 position) {
            NGNode node = new NGNode();
            node.Initialize(position);
            node.Draw();

            return node;
        }

        private void AddGridBackground() {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles() {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("DungeonSystem/DungeonNodeGraphStyle.uss");

            styleSheets.Add(styleSheet);
        }

        private void AddManipulators() {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger()); // HA D'ANAR SEMPRE ABANS DEL RECTANGLE SELECTOR!!!
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu());
        }

        private IManipulator CreateNodeContextualMenu() {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)))
                );
            return contextualMenuManipulator;
        }
    }
}
