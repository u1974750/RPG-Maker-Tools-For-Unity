using NG.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    [Tooltip("The name of the level")] public string levelname;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    [Tooltip("Populate the list with the room templates that you want to be part of the level")]
    public List<RoomTemplateSO> roomTemplateList;

    //[Space(10)][Header("ROOM NODE GRAPHS FOR LEVEL")] 

}
