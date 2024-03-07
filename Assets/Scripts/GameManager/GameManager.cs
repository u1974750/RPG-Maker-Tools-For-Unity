using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Space(10)]
    [Header("DUNGEON LEVELS")]

    [Tooltip("Populate with the dungeon level scriptable objects")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [Tooltip("Populate with the strarting dungeon level for testing, first level = 0")]
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState gameState;



    // Start is called before the first frame update
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        //FOR TESTING!!         
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameState.gameStarted;
        }
    }

    private void HandleGameState() {
        switch (gameState) {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex) {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);
        if (!dungeonBuiltSuccessfully) {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }
    }


    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this,nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
