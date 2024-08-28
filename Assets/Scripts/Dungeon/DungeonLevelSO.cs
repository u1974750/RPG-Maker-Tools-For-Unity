using NG.Elements;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject {
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    [Tooltip("The name of the level")] public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    [Tooltip("Populate the list with the room templates that you want to be part of the level")]
    public List<RoomTemplateSO> roomTemplateList;

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR

    //COMPROVA QUE ELS ELEMENTS ESTIGUIN ENTRATS CORRECTAMENT AL SO
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        //valida que els room templates estiguin especificats

        //coirridors
        bool isHorizontalCorridor = false;
        bool isVerticalCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {
            if (roomTemplate == null) return;
            if (roomTemplate.roomNodeType.isCorridorEW) isHorizontalCorridor = true;
            if (roomTemplate.roomNodeType.isCorridorNS) isVerticalCorridor = true;
            if (roomTemplate.roomNodeType.isEntrance) isEntrance = true;
        }

        if (isHorizontalCorridor == false) {
            //MISSATGE ERROR FALTA HORIZONTAL CORRIDOR!!!
            Debug.LogWarning("FALTA PASSADIS HORITZONTAL");
        }
        if (isVerticalCorridor == false) {
            //MISSATGE ERROR FALTA VERTICAL CORRIDOR!!! 
            Debug.LogWarning("FALTA PASSADIS VERTICAL");
        }
        if (isEntrance == false) {
            //MISSATGE ERROR FALTA ENTRANCE !!! 
            Debug.LogWarning("FALTA ENTRADA ");
        }

        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList) {
            if (roomNodeGraph == null) return;

            foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList) {
                if (roomNode == null) continue;

                //check that a room template has been specified for each roomNode type

                //corridors and entrance already checked
                if (roomNode.roomNodeType.isEntrance || roomNode.roomNodeType.isCorridorEW || roomNode.roomNodeType.isCorridorNS
                    || roomNode.roomNodeType.isCorridor || roomNode.roomNodeType.isNone) {
                    continue;
                }

                bool isRoomNodeTypeFound = false;
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList) {
                    if (roomTemplateSO == null) continue;

                    if (roomTemplateSO.roomNodeType == roomNode.roomNodeType) {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound) Debug.LogWarning(" There is no Room Template found for the node graph");
            }

        }
    }

#endif
    #endregion Validation



}
