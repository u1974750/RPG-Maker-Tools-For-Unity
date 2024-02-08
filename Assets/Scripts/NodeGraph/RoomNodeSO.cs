using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string _id;
    [HideInInspector] public List<string> _parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> _childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO _roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO _roomNodeTypeList;
    public RoomNodeTypeSO _roomNodeType;

}
