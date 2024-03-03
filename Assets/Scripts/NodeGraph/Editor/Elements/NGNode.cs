using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NG.Elements {
    public class NGNode : Node {
        public string RoomName { get; set; }
        public List<string> Choices { get; set; }
        public Image thumbnail {  get; set; }


        public virtual void Initialize(Vector2 position) {
            RoomName = "RoomName";
            Choices = new List<string>();
            thumbnail = new Image();
            SetPosition(new Rect(position, Vector2.zero));
        }

        public virtual void Draw() {

            // TITLE CONTAINER
            TextField roomNameTextField = new TextField() {
                value = RoomName
            };

            titleContainer.Insert(0, roomNameTextField);

            //INPUT CONTAINER            
            Port inputPort_N = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));//DIRECTION; SI ES INPUT O OUTPUT -- CAPACITY: si es pot conectar a un o mes nodes
            inputPort_N.portName = "Previous Room - North";
            inputContainer.Add(inputPort_N);

            Port inputPort_S = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort_S.portName = "Previous Room - South";
            inputContainer.Add(inputPort_S);

            Port inputPort_E = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort_E.portName = "Previous Room - East";
            inputContainer.Add(inputPort_E);

            Port inputPort_W = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort_W.portName = "Previous Room - West";
            inputContainer.Add(inputPort_W);


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

            Image image = new Image() {
                image = EditorGUIUtility.Load("node1") as Texture2D
            };
            customDataContainer.Add(image);


            extensionContainer.Add(customDataContainer);
            

           
        }
    }

}
