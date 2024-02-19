using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NG.Elements {
    public class NGNode : Node
    {
        public string RoomName { get; set; }
        public List<string> Choices { get; set; }


        public void Initialize(Vector2 position) {
            RoomName = "RoomName";
            Choices = new List<string>();

            SetPosition(new Rect(position, Vector2.zero));
        }

        public void Draw() {

            // TITLE CONTAINER
            TextField roomNameTextField = new TextField() {
                value = RoomName
            };

            titleContainer.Insert(0, roomNameTextField);

            //INPUT CONTAINER            
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));//DIRECTION; SI ES INPUT O OUTPUT -- CAPACITY: si es pot conectar a un o mes nodes
            inputPort.portName = "Previous Room";
            inputContainer.Add(inputPort);


            //EXTENSION CONTAINER

            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout() {
                text = "hello!!"
            };
            TextField textTextField = new TextField() {
                value = "this is my sample text"
            };
            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }

}
