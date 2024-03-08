using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
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
    /// Disable collision tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer() {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
