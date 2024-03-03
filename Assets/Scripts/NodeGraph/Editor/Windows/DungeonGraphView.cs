using NG.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NG.Windows {
    using Enumerations;
    public class DungeonGraphView : GraphView
    {
        public DungeonGraphView() {
            AddManipulators();
            AddGridBackground();
            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            List <Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {
                if(startPort == port) {
                    return;
                }
                if(startPort.node == port.node) {
                    return;
                }
                if(startPort.direction == port.direction) {
                    return;
                }

                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private NGNode CreateNode(NGRoomType roomType, Vector2 position) {
            //Type nodeType = Type.GetType($"NG.Elements.NG{roomType}Node");
            //IMPORTANT EL NODE S'HA DE DIR DE MANERA ESPECIFICA PER AL DE TENIR-NE VARIS FUNCIONALS!

            Type nodeType = Type.GetType($"NG.Elements.NGRoomNode");

            NGNode node = Activator.CreateInstance(nodeType) as NGNode;

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
            this.AddManipulator(CreateNodeContextualMenu("Add Node (LargeRoom)", NGRoomType.LargeRoom));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (BossRoom)", NGRoomType.BossRoom));
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, NGRoomType roomType) {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(roomType, actionEvent.eventInfo.localMousePosition)))
                );
            return contextualMenuManipulator;
        }
    }
}
