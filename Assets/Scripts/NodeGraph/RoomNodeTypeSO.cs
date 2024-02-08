using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string _roomNodeTypeName;
    public bool _displayInNodeGraphEditor;
    public RoomType _roomType;

    public enum RoomType
    {
        Corridor,
        CorridorVertical,
        CorridorHorizontal,
        Entrance,
        BossRoom,
        None
    }

}
