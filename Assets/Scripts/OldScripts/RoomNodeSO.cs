using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class RoomNodeSO : Node
{
    [HideInInspector] public string _id;
    [HideInInspector] public List<string> _parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> _childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO _roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO _roomNodeTypeList;
    public RoomNodeTypeSO _roomNodeType;


    public void Initialize() {
        _id = "This is the ID";
    }
}
