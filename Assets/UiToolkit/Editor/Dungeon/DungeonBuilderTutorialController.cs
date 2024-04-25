using NG.Elements;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DungeonBuilderTutorialController : EditorWindow
{
    [SerializeField] private StyleSheet _styleSheet;
    [MenuItem("Dungeon Builder/Open Tile Palette")]
    public static void OpenTilePalette() {
        EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
    }

    [MenuItem("Dungeon Builder/Open Room prefab Editor")]
    public static void DuplicateRoomPrefab() {
        GameObject roomTemplatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Dungeon/Rooms/RoomTemplate.prefab");
        // Duplicate the prefab
        GameObject duplicatedPrefab = PrefabUtility.InstantiatePrefab(roomTemplatePrefab) as GameObject;


        // Check if duplication was successful
        if (duplicatedPrefab != null) {

            PrefabUtility.SaveAsPrefabAsset(duplicatedPrefab, "Assets/Prefabs/Dungeon/Rooms/NewRoom.prefab");
            // Open the duplicated prefab for editing
            DestroyImmediate(duplicatedPrefab);
            duplicatedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Dungeon/Rooms/NewRoom.prefab");
            Instantiate(duplicatedPrefab);
            Selection.activeGameObject = duplicatedPrefab;
            EditorGUIUtility.PingObject(duplicatedPrefab);
            OpenTilePalette();

        }
        else {
            Debug.LogWarning("Failed to duplicate prefab.");
        }
    }

    [MenuItem("Dungeon Builder/ TUTORIAL")]
    public static void OpenTutorialWindow() {
        DungeonBuilderTutorialController wnd = GetWindow<DungeonBuilderTutorialController>();
        wnd.titleContent = new GUIContent("Room Builder Tutorial");
    }

    public void CreateGUI() {
        VisualElement _root;
        _root = rootVisualElement;

        if(_styleSheet != null) {  _root.styleSheets.Add(_styleSheet); }

        Label mainTitle = new Label("~ ROOM BUILDER TUTORIAL ~");
        mainTitle.AddToClassList("main-title");
        _root.Add(mainTitle);

        ScrollView _scrollView = new ScrollView(ScrollViewMode.Vertical);
        _scrollView.SetEnabled(true);
        _root.Add(_scrollView);

        TextElement introText = new TextElement() { text = "Welcome to the ROOM BUILDER TOOL!" };
        introText.AddToClassList("simple-text");
        _scrollView.Add(introText);

        TextElement text1 = new TextElement(){text = "In here you can follow step by step the tutorial, " +
                                                     "ending up with a custom dungeon level!"};
        text1.AddToClassList("simple-text");
        _scrollView.Add(text1);


        FirstBox(_scrollView);
        SecondBox(_scrollView);
        ThirdBox(_scrollView);
        FourthBox(_scrollView);
        FifthBox(_scrollView);
    }

    private void FirstBox(ScrollView sv) {
        Box box = new Box();
        box.AddToClassList("tutorial-box");

        Label titleLabel = new Label("1. CREATE THE ROOM");
        titleLabel.AddToClassList("subtitle-label");
        box.Add(titleLabel);

        TextElement txt1 = new TextElement() {
            text = "First things first, press this button to create a Room Prefab." +
                                                      "Open the prefab, and you will see different players already prepared." +
                                                      "Use the Tile Palette to create your room however you like!"
        };
        txt1.AddToClassList("simple-text");
        box.Add(txt1);

        HelpBox txt2 = new HelpBox("Remember to delete the prefab from the scene once you are finished!!", HelpBoxMessageType.None);
        txt2.AddToClassList("tutorial-small-helpbox");
        box.Add(txt2);

        Button btn1 = new Button(DuplicateRoomPrefab) { text = "Create Room Prefab"};
        box.Add(btn1);

        sv.Add(box);
    }

    private void SecondBox(ScrollView sv) {
        Box box = new Box();
        box.AddToClassList("tutorial-box");

        Label titleLabel = new Label("2. CREATE THE S.O. ELEMENT");
        titleLabel.AddToClassList("subtitle-label");
        box.Add(titleLabel);


        TextElement txt2 = new TextElement() {
            text = "Once you finish \"drawing\" your room, you have to create the S.O, so the project knows how to connect " +
                   "your room with the others.\n" +
                   "You can use the Default Scriptable Object, or the Dungeon Room Creator tool, " +
                   "which makes it easier and explains each step."
        };
        txt2.AddToClassList("simple-text");
        box.Add(txt2);

        HelpBox txt1 = new HelpBox("S.O. means Scriptable Object, you can use the default scriptable " +
                                   "object created for this project or fill the Room Builder tool to auto create it", 
                                   HelpBoxMessageType.None);
        txt1.AddToClassList("tutorial-small-helpbox");
        box.Add(txt1);

        Button btn1 = new Button(RoomBuilder.OpenRoomBuilderWindow) { text = "Room Builder Tool" };
        box.Add(btn1);

        Button btn2 = new Button(DuplicateRoomSO) { text = "Room Scriptable Object" };
        box.Add(btn2);

        sv.Add(box);
    }

    private void ThirdBox(ScrollView sv) {
        Box box = new Box();
        box.AddToClassList("tutorial-box");

        Label titleLabel = new Label("3. DEFINE THE DUNGEON ROOM GRAPH");
        titleLabel.AddToClassList("subtitle-label");
        box.Add(titleLabel);

        TextElement txt1 = new TextElement() {
            text = "If you only have one room, you might want to repeat steps 1 and 2 a few times.\n" +
                   "Now that you have different rooms created, it's time to create a level.\n" +
                   "With the next button a Node editor will open, with right click you will be able to spawn a node" +
                   "and select which type it corresponds to. Connect them until you are happy with how it looks"
        };
        txt1.AddToClassList("simple-text");
        box.Add(txt1);

        HelpBox txt2 = new HelpBox("The orientation of the nodes doesn't affect the final dungeon, if you place a room " +
                                   "at the right of another, it might create on the left!",
                                   HelpBoxMessageType.None);
        txt2.AddToClassList("tutorial-small-helpbox");
        box.Add(txt2);
        
        HelpBox txt3 = new HelpBox("You can create multiple rooms of the same type, for example various Small Rooms, later on " +
                                   "you will select which ones you want for this specific node graph",
                                   HelpBoxMessageType.None);
        txt3.AddToClassList("tutorial-small-helpbox");
        box.Add(txt3);

        Button btn1 = new Button(DuplicateRoomNodeGraph) { text = "New Node Graph" };
        box.Add(btn1);


        sv.Add(box);
    }

    private void FourthBox(ScrollView sv) {
        Box box = new Box();
        box.AddToClassList("tutorial-box");

        Label titleLabel = new Label("4. CREATE THE LEVEL");
        titleLabel.AddToClassList("subtitle-label");
        box.Add(titleLabel);

        TextElement txt1 = new TextElement() {
            text = "Now it's time to create the level! You can have multiple Room Node Graphs " +
                   "if you want more generation variety\n\n" +
                   "Press the button below to create a new level S.O and populate the variables:\n" +
                   "1. Enter the name you like for the level. \n" +
                   "2. Enter the different rooms S.O you created previously. " +
                       "Select only the ones you want to use for this specific level!\n" +
                   "3. Enter the node graph (or multiple node graphs) that you created for this level"
        };
        txt1.AddToClassList("simple-text");
        box.Add(txt1);

        Button btn1 = new Button(DuplicateLevelSO) {
            text = "New Level S.O.",
            style = { marginTop = 5f, marginBottom = 2f }
        };
        box.Add(btn1);

        HelpBox txt3 = new HelpBox("If you can't find your RoomS.O. Folder or the Node graph folder use the buttons below!",
                                   HelpBoxMessageType.None);
        txt3.AddToClassList("tutorial-small-helpbox"); ;
        box.Add(txt3);

        Button btn2 = new Button(PingNodeGraphFolder) { text = "Locate NodeGraph Folder" };
        box.Add(btn2);
        Button btn3 = new Button(PingRoomSOFolder) { text = "Locate Room S.O. Folder" };
        box.Add(btn3);

        sv.Add(box);
    }

    private void FifthBox(ScrollView sv) {
        Box box = new Box();
        box.AddToClassList("tutorial-box");

        Label titleLabel = new Label("5. GAME MANAGER");
        titleLabel.AddToClassList("subtitle-label");
        box.Add(titleLabel);

        TextElement txt1 = new TextElement() {
            text = "Great, we are almost done!\n" +
                   "on the scene inspector you will find the GameManager object, inside there is a field called " +
                   "\"Dungeon Level List\", you can drag there the level you just created. The order will indicate the game level orders." +
                   "If you want to test it, make sure the \"Current Dungeon Level\" is set to the same number as your level element number."
        };
        txt1.AddToClassList("simple-text");
        box.Add(txt1);

        HelpBox txt3 = new HelpBox("If somehow you deleted the Game manager from the scene, you could find a new one inside the prefab/Resources folder, " +
                                    "or use the button below",
                                  HelpBoxMessageType.None);
        txt3.AddToClassList("tutorial-small-helpbox"); ;
        box.Add(txt3);

        Button btn3 = new Button(NewGameManager) { text = "Spawn new GameManager" };
        box.Add(btn3);

        sv.Add(box);

    }

    private void DuplicateRoomSO() {
        RoomTemplateSO newRoom = ScriptableObject.CreateInstance<RoomTemplateSO>();
        AssetDatabase.CreateAsset(newRoom, "Assets/ScriptableObjectAssets/Dungeon/Rooms/Room_newRoom.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRoom;
    }

    private void DuplicateRoomNodeGraph() {
        RoomNodeGraphSO newGraph = ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        AssetDatabase.CreateAsset(newGraph, "Assets/ScriptableObjectAssets/Dungeon/RoomNodeGraphs/NodeGraph_newNodeGraph.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        RoomNodeGraphEditor.OnDoubleClickAsset(newGraph.GetInstanceID(), 0);
    }

    private void DuplicateLevelSO() {
        DungeonLevelSO newLevel = ScriptableObject.CreateInstance<DungeonLevelSO>();
        AssetDatabase.CreateAsset(newLevel, "Assets/ScriptableObjectAssets/Dungeon/Levels/DungeonLevel_newLevel.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
    }

    private void PingNodeGraphFolder() {
        string path = "Assets/ScriptableObjectAssets/Dungeon/RoomNodeGraphs";
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        EditorGUIUtility.PingObject(obj);
    }

    private void PingRoomSOFolder() {
        string path = "Assets/ScriptableObjectAssets/Dungeon/Rooms";
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        EditorGUIUtility.PingObject(obj);
    }

    private void NewGameManager() {
        if(GameObject.Find("GameManager") == null) {
            GameObject prefab = Resources.Load<GameObject>("GameManager");
            if(prefab != null) {
                GameObject instantiatedPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
            else {
                Debug.Log("Error getting the prefab");
            }
        }
        else {
            Debug.Log("You already have one Game Manager!");
        }

    }
}

