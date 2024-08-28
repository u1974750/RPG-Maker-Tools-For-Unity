using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour {
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap wallsTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
    }

    /// <summary>
    /// Initialise the instantiated Room
    /// </summary>
    public void Initialise(GameObject roomGameobject) {
        PopulateTilemapMemberVariables(roomGameobject);
        BlockOffUnusedDoorWays();
        DisableCollisionTilemapRenderer();

    }

    /// <summary>
    /// Populate the tilemap and grid member variables
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject) {
        grid = roomGameobject.GetComponentInChildren<Grid>(); //get the grid component
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>(); // get tilemaps in children

        foreach (Tilemap tilemap in tilemaps) {
            switch (tilemap.gameObject.tag) {
                case "wallsTilemap": wallsTilemap = tilemap; break;
                case "groundTilemap": groundTilemap = tilemap; break;
                case "decoration1Tilemap": decoration1Tilemap = tilemap; break;
                case "decoration2Tilemap": decoration2Tilemap = tilemap; break;
                case "frontTilemap": frontTilemap = tilemap; break;
                case "collisionTilemap": collisionTilemap = tilemap; break;
                default: break;
            }
        }
    }

    /// <summary>
    /// Block off unused doorways in the room
    /// </summary>
    private void BlockOffUnusedDoorWays() {
        foreach (Doorway doorway in room.doorwayList) {
            if (doorway.isConnected) {
                continue;
            }

            if (collisionTilemap != null) {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (groundTilemap != null) {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (wallsTilemap != null) {
                BlockADoorwayOnTilemapLayer(wallsTilemap, doorway);
            }
            if (decoration1Tilemap != null) {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null) {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null) {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block a doorway on a tilemap layer
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway) {
        switch (doorway.orientation) {
            case RoomOrientation.north:
            case RoomOrientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case RoomOrientation.east:
            case RoomOrientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case RoomOrientation.none: break;
        }
    }

    /// <summary>
    /// Block doorway horizontally (north and south doors)
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway) {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)); //Get rotation of tile being copied

                //copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0),
                                               tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block doorway vertically (east and west doors)
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway) {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)); //Get rotation of tile being copied

                //copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0),
                                               tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }


    /// <summary>
    /// Disable collision tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer() {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
