using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NG.Elements {
    public class NGRoomNode : NGNode {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            RoomName = "Room Node";

            Choices.Add("Next Room - N");
            Choices.Add("Next Room - S");
            Choices.Add("Next Room - E");
            Choices.Add("Next Room - W");
        }

        public override void Draw() {
            base.Draw();

            //OUTPUT CONTAINER
            foreach(string choice in Choices) {
                Port roomPort = InstantiatePort(Orientation.Horizontal,
                                                Direction.Output,
                                                Port.Capacity.Single,
                                                typeof(bool));

                roomPort.portName = choice;

                outputContainer.Add(roomPort);
            }
            RefreshExpandedState();
        }


    }

}

