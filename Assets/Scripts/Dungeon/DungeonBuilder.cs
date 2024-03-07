using NG.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string,Room> dungeonBuilderRoomDictionary = new Dictionary<string,Room>();
    
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string,RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    protected override void Awake() {
        base.Awake();
        
        LoadRoomNodeTypeList();//Load the room node type list
       
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f); //Set dimmed material to fully visible
    }

    /// <summary>
    /// Load the room node type list
    /// </summary>
    private void LoadRoomNodeTypeList() {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Generate random dungeon
    /// </summary>
    /// <returns>True if dungeon built. False if failed</returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel) {
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        LoadRoomTemplatesIntoDuctionary(); // Load the scriptable object room templates into the dictionary

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while(!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts) {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList); //Select random room node graph from the list

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            //Loop until dungeon successfully built or more than max attempts for node graph
            while(!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph) {
                ClearDungeon();
                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph); // Attempt to Build a random dungeon for the selected node graph
            }

            if(dungeonBuildSuccessful ) {
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Instantiate the dungeon room gameobjects from the prefabs
    /// </summary>
    private void InstantiateRoomGameobjects() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Load the room templates into the dictionary
    /// </summary>
    private void LoadRoomTemplatesIntoDuctionary() {
        roomTemplateDictionary.Clear();

        foreach(RoomTemplateSO roomTemplate in roomTemplateList) {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid)) {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else {
                Debug.Log("Duplicate Room Template Key In" + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    /// <returns>Random node graph from the list</returns>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList) {
        if(roomNodeGraphList.Count > 0) {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else {
            Debug.Log("No Room node Graphs in list");
            return null;
        }
    }

    /// <summary>
    /// Clear dungeon room gameObjects and dungeon room dictionary
    /// </summary>
    private void ClearDungeon() {
        if(dungeonBuilderRoomDictionary.Count > 0) {
            foreach(KeyValuePair<string,Room> keyvaluepair in dungeonBuilderRoomDictionary) {
                Room room = keyvaluepair.Value;
                if(room.instantiatedRoom != null) {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified nodeGraph
    /// </summary>
    /// <returns>True if a successful random layout was generated. False if a problem was encountered and another attempt is required</returns>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph) {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>(); //Create Open Room Node Queue
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance)); //Add entrance node to room node queue from Room Node Graph

        if(entranceNode != null) {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else {
            Debug.LogError("NO ENTRANCE NODE!");
            return false;
        }

        
        bool noRoomOverlaps = true; //start with no room overlaps
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if(openRoomNodeQueue.Count == 0 & noRoomOverlaps) {
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Process rooms in the open room node queue
    /// </summary>
    /// <returns>True if there are no room overlaps</returns>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps) {
        // While( room nodes in ropen room node queue && No room overlaps detected)
        while(openRoomNodeQueue.Count > 0 && noRoomOverlaps == true) {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();
            
            //Add child nodes to queue from room node graph
            foreach(RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode)) {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode.roomNodeType.isEntrance) {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
                room.isPositioned = true;

                //Add room ro room dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else { //RoomType isn't an entrance
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// Attempt to place the room node in the dungeon
    /// </summary>
    /// <returns>If room can be placed return the room, else return null</returns>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom) {
        bool roomOverlaps = true;

        while(roomOverlaps) {
            List<Doorway> unconnectedAvailavleParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if(unconnectedAvailavleParentDoorways.Count == 0) {
                return false; //if thera are no more doorways to try then overlap failure
            }

            Doorway doorwayParent = unconnectedAvailavleParentDoorways[UnityEngine.Random.Range(0,unconnectedAvailavleParentDoorways.Count)];
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent); // consistent door orientation!

            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode); //Create a room

            if(PlaceTheRoom(parentRoom, doorwayParent, room)) { // Place the room (true if it doesn't overlap)
                roomOverlaps = false;
                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else {
                roomOverlaps = true;
            }
        }
        return true;
    }

    /// <summary>
    /// Place the Room 
    /// </summary>
    /// <returns>True if the room doesn't overlap. False otherwise</returns>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room) {
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);
        if(doorway == null) {
            doorwayParent.isUnavailable = true;
            return false;
        }

        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;
        switch(doorway.orientation) {
            case RoomOrientation.North: adjustment = new Vector2Int( 0,-1); break;
            case RoomOrientation.South: adjustment = new Vector2Int( 0, 1); break;
            case RoomOrientation.East:  adjustment = new Vector2Int(-1, 0); break;
            case RoomOrientation.West:  adjustment = new Vector2Int( 1, 0); break;
            case RoomOrientation.none: break;
            default: break;
        }
        
        //calculate room lower bounds and upper bounds
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if(overlappingRoom == null) {
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            return true;
        }
        else {
            doorwayParent.isUnavailable = true;
            return false;
        }
         
    }

    /// <summary>
    /// Check for rooms that overlap the upper and lower bounds parameters
    /// </summary>
    /// <returns>if there are overlapping rooms then return room, else return null</returns>
    private Room CheckForRoomOverlap(Room roomToTest) {
        foreach(KeyValuePair<string,Room> keyvaluepair in dungeonBuilderRoomDictionary) {
            Room room = keyvaluepair.Value;

            if(room.id == roomToTest.id || !room.isPositioned) { //skip if room as same room to thest or hasn't been positioned
                continue;
            }

            if(IsOverLappingRoom(roomToTest, room)) {
                return room;
            }
        }
        return null;
    }

    /// <summary>
    /// Check if 2 rooms overlap each other
    /// </summary>
    /// <returns> True if they overlap or false if they don't overlap</returns>
    private bool IsOverLappingRoom(Room room1, Room room2) {
        bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if(isOverlappingX && isOverlappingY) {  return true; }
        else {  return false; }
    }

    /// <summary>
    /// Check if interval 1 overlaps interval 2
    /// </summary>
    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2) {
        if(Mathf.Max(imin1,imin2) <= Mathf.Min(imax1, imin2)) {
            return true;
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Get the doorway from the doorwayList that has the opposite orientation to doorway
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList) {
        foreach(Doorway doorwayToCheck in doorwayList) {
            if(parentDoorway.orientation == RoomOrientation.East && doorwayToCheck.orientation == RoomOrientation.West) {
                return doorwayToCheck;
            }
            else if(parentDoorway.orientation == RoomOrientation.West && doorwayToCheck.orientation == RoomOrientation.East) {
                return doorwayToCheck;
            }
            else if(parentDoorway.orientation == RoomOrientation.North && doorwayToCheck.orientation == RoomOrientation.South) {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == RoomOrientation.South && doorwayToCheck.orientation == RoomOrientation.North) {
                return doorwayToCheck;
            }
        }
        return null;    
    }

    /// <summary>
    /// Get random room template for room node taking account the parent doorway orientation
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent) {
        RoomTemplateSO roomTemplate = null;

        if(roomNode.roomNodeType.isCorridor) {

            switch(doorwayParent.orientation) {

                case RoomOrientation.North:
                case RoomOrientation.South:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case RoomOrientation.East:
                case RoomOrientation.West:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                default:
                    break;

            }
        }
        else {
            roomTemplate = GetRandomRoomTemplate(roomTemplate.roomNodeType);
        }

        return roomTemplate;
    }

    /// <summary>
    /// Get Random Room Template from the roomTemplateList that matches the roomType and return it
    /// </summary>
    /// <returns>RoomTemplate from the room template list that matches the room type. Null if no marching room templates found</returns>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType) {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        //Loop through room template list 
        foreach (RoomTemplateSO roomTemplate in roomTemplateList) {
            if (roomTemplate.roomNodeType == roomNodeType) {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0) {
            return null;
        }
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)]; //Return random matching room template
    }

    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList) {
        foreach(Doorway doorway in roomDoorwayList) {
            if(!doorway.isConnected && !doorway.isUnavailable) {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// Create room based on roomTemplate and layoutNode
    /// </summary>
    /// <returns>Created Room</returns>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode) {
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList); //copy List with a deep copy to avoid referenced list
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList); //copy List with a deep copy to avoid referenced list

        if(roomNode.parentRoomNodeIDList.Count == 0) { //Entrance
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
        }
        else {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }
        return room;
    }

    /// <summary>
    /// Create deep copy of doorway list
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList) {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach(Doorway doorway in oldDoorwayList) {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }
        return newDoorwayList;
    }

    /// <summary>
    /// Creates a deep copy of string list
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList) {
        List<string> newStringList = new List<string>();

        foreach(string stringValue in oldStringList) {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

    /// <summary>
    /// Get a room template by room template ID
    /// </summary>
    /// <returns>null if ID desn't exists</returns>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID) {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate)) return roomTemplate;
        else return null;
    }

    /// <summary>
    /// Get Room By Room ID
    /// </summary>
    /// <returns> If no room exists with that id return null</returns>
    public Room GetRoomByRoomID(string roomID) {
        if(dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room)) return room;
        else return null;
    }
}
