using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    [Tooltip("Populate with all the RoomNodeTypeSO created!!")]
    public List<RoomNodeTypeSO> _list;
}
