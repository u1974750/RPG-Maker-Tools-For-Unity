using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NG.Elements {

    [CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
    public class RoomTemplateSO : ScriptableObject {
        [HideInInspector] public string guid;

        [Space(10)]
        [Header("ROOM PREFAB")]

        [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]
        public GameObject prefab;

        [HideInInspector] public GameObject previousPrefab; //used to regenerate the guid if the so is copied and the prefab is changed

        [Space(10)]
        [Header("ROOM CONFIGURATION")]

        [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph.  The exceptions being with corridors.  In the room node graph there is just one corridor type 'Corridor'.  For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]
        public RoomNodeTypeSO roomNodeType;


        [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position")]
        public Vector2Int lowerBounds;


        [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position")]
        public Vector2Int upperBounds;


        [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'")]
        [SerializeField] public List<Doorway> doorwayList;


        [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]
        public Vector2Int[] spawnPositionArray;

        /// <summary>
        /// Returns the list of Entrances for the room template
        /// </summary>
        public List<Doorway> GetDoorwayList() {
            return doorwayList;
        }

        #region Validation

#if UNITY_EDITOR

        // Validate SO fields
        private void OnValidate() {
            // Set unique GUID if empty or the prefab changes
            if (guid == "" || previousPrefab != prefab) {
                guid = GUID.Generate().ToString();
                previousPrefab = prefab;
                EditorUtility.SetDirty(this);
            }
        }

#endif

        #endregion Validation
    }
}
