using NG.Elements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomBuilder : EditorWindow {

    //private VisualTreeAsset _VisualTreeAsset = default;
    private VisualElement _root;
    private List<HelpBox> helpBoxes = new List<HelpBox>();
    private ScrollView _scrollView;
    private Foldout _foldout;
    private UnityEngine.Object _roomPrefab;
    private TabView _doorwaysTab;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private Texture2D _imagePlaceholder;
    [SerializeField] private Texture2D _doorSprite;

    [MenuItem("Dungeon Builder/Room Builder")]
    public static void OpenRoomBuilderWindow() {
        RoomBuilder wnd = GetWindow<RoomBuilder>();
        wnd.titleContent = new GUIContent("RoomBuilder");
    }


    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        _root = rootVisualElement;

        if (_styleSheet != null) {
            _root.styleSheets.Add(_styleSheet);
        }

        //MAIN TITLE
        Label mainTitle = new Label("~ ROOM BUILDER ~");
        mainTitle.AddToClassList("main-title");
        _root.Add(mainTitle);

        //TOGGLE FOR THE TUTORIAL
        TutorialToggle();

        //ADD SCROLLER
        _scrollView = new ScrollView(ScrollViewMode.Vertical);
        _scrollView.SetEnabled(true);
        _root.Add(_scrollView);


        //CreateRoomPrefabFoldout();

        
        RoomTemplateFoldout();
        Doorways(_scrollView);
        ExampleImages(_scrollView);

        //CREATE SCRIPTABLE OBJECT
        Button createObjectButton = new Button(CreateAssetButtonAction) { text = "Create Room" };
        _root.Add(createObjectButton);

    }

    private void CreateAssetButtonAction() {
        List<VisualElement> childrenList = _foldout.Children().ToList();

        RoomTemplateSO newRoom = ScriptableObject.CreateInstance<RoomTemplateSO>();
        TextField inputName = (TextField)childrenList[0];
        string path = "Assets/ScriptableObjectAssets/Dungeon/Rooms/" + inputName.value + ".asset";

        newRoom.prefab = (GameObject)_roomPrefab;

        var roomTypeInput = (PopupField<RoomNodeTypeSO>)childrenList[4];
        newRoom.roomNodeType = roomTypeInput.value;

        Vector2IntField lowerBoundInput = (Vector2IntField)childrenList[6];
        newRoom.lowerBounds = lowerBoundInput.value;

        Vector2IntField upperBoundInput = (Vector2IntField)childrenList[8];
        newRoom.upperBounds = upperBoundInput.value;


        newRoom.doorwayList = new List<Doorway>();

        for (int i = 0; i < 4; i++) {
            VisualElement doorWay = _doorwaysTab.Children().ToList()[i];
            Doorway newDoorway = new Doorway();

            List<VisualElement> doorChildren = doorWay.Children().ToList();

            EnumField doorOrientation = (EnumField)doorChildren[1];
            newDoorway.orientation = (RoomOrientation)doorOrientation.value;

            Vector2IntField doorPos = (Vector2IntField)doorChildren[3];
            newDoorway.position = doorPos.value;

            Vector2IntField startCopyPos = (Vector2IntField)doorChildren[5];
            newDoorway.doorwayStartCopyPosition = startCopyPos.value;

            IntegerField tileWidth = (IntegerField)doorChildren[7];
            newDoorway.doorwayCopyTileWidth = tileWidth.value;

            IntegerField tileHeight = (IntegerField)doorChildren[9];
            newDoorway.doorwayCopyTileHeight = tileHeight.value;

            newRoom.doorwayList.Add(newDoorway);
        }


        AssetDatabase.CreateAsset(newRoom, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRoom;


    }

    private void Doorways(ScrollView scrollView) {
        Foldout roomTemplateFoldout = new Foldout { text = "2. Doorways" };

        var box = new Box();
        box.AddToClassList("doorways-box");

        var DoorwaysTab = new TabView() { style = { marginTop = 15 } };

        for (int i = 0; i < 4; i++) {
            var door = new Tab("Door " + ( i + 1 ), _doorSprite);

            HelpBox doorHelper = new HelpBox("There should be four doorways for a room" +
                " - one for each compass direction.  These should have a consistent 3 tile opening size," +
                " with the middle tile position being the doorway coordinate 'position'", HelpBoxMessageType.Info) { style = { marginTop = 10 } };
            helpBoxes.Add(doorHelper);
            door.Add(doorHelper);


            EnumField doorOrientation = new EnumField("Door Orientation", RoomOrientation.none) { style = { marginTop = 10 } };
            door.Add(doorOrientation);

            HelpBox doorPosHelpBox = new HelpBox("The door position is the center of the door, at the outside of the room." +
                                                " This wil be used to connect this room with the next", HelpBoxMessageType.Info);
            helpBoxes.Add(doorPosHelpBox);
            door.Add(doorPosHelpBox);

            Vector2IntField doorPos = new Vector2IntField("Door Position") { style = { marginTop = 10 } };
            door.Add(doorPos);

            HelpBox copyPositionHelp = new HelpBox("If the door isn't used the programm will cover it up with a wall." +
                                                   "The start copy position is the tile from where it starts to copy, in horizontal is the left, in vertical the top one", HelpBoxMessageType.Info);
            helpBoxes.Add(copyPositionHelp);
            door.Add(copyPositionHelp);

            Vector2IntField startCopyField = new Vector2IntField("Start Copy Position") { style = { marginTop = 10 } };
            door.Add(startCopyField);

            HelpBox tileWHelpBox = new HelpBox("Width is the number of tiles to cover horizontally", HelpBoxMessageType.Info);
            helpBoxes.Add(tileWHelpBox);
            door.Add(tileWHelpBox);

            IntegerField copyWidth = new IntegerField("Copy Tiles Width") { style = { marginTop = 10 } };
            door.Add(copyWidth);

            HelpBox tileHHelpBox = new HelpBox("Height is the number of tiles to cover vertically", HelpBoxMessageType.Info);
            helpBoxes.Add(tileHHelpBox);
            door.Add(tileHHelpBox);

            IntegerField copyHeight = new IntegerField("Copy Tiles Height") { style = { marginTop = 10 } };
            door.Add(copyHeight);

            DoorwaysTab.Add(door);

        }

        box.Add(DoorwaysTab);
        roomTemplateFoldout.Add(box);
        scrollView.Add(roomTemplateFoldout);
        _doorwaysTab = DoorwaysTab;
    }

    private void ExampleImages(ScrollView scrollView) {
        //create foldout
        Foldout exampleImagesFoldout = new Foldout { text = "3. Example Images" };

        //create box
        var box = new Box();
        box.AddToClassList("doorways-box");

        //tab view
        var categoriesTab = new TabView() { style = { marginTop = 15 } };

        //tabs
        #region tab1
        var tab1 = new Tab("Front Door");

        Image image1 = new Image();

        Sprite aux = Resources.Load<Sprite>("DungeonBuilderImages/FrontDoor");
        if (aux == null) {
            Debug.Log("cannot find the image");
            return;
        }
        image1.sprite = aux;
        tab1.Add(image1);
        categoriesTab.Add(tab1);
        #endregion

        #region tab2
        var tab2 = new Tab("Side Door");

        Image image2 = new Image();

        aux = Resources.Load<Sprite>("DungeonBuilderImages/SideDoor");
        if (aux == null) {
            Debug.Log("cannot find the image");
            return;
        }
        image2.sprite = aux;
        tab2.Add(image2);
        categoriesTab.Add(tab2);
        #endregion

        #region tab3
        var tab3 = new Tab("Room Limit");

        Image image3 = new Image();

        aux = Resources.Load<Sprite>("DungeonBuilderImages/RoomLimit");
        if (aux == null) {
            Debug.Log("cannot find the image");
            return;
        }
        image3.sprite = aux;
        tab3.Add(image3);
        categoriesTab.Add(tab3);
        #endregion

        //add to box and scrollview
        box.Add(categoriesTab);
        exampleImagesFoldout.Add(box);
        scrollView.Add(exampleImagesFoldout);


    }

    private void RoomTemplateFoldout() {
        Foldout roomTemplateFoldout = new Foldout { text = "1. Room Template", style = { marginRight = 15 } };

        //room name
        TextField roomName = new TextField() { label = "Room Name: ", style = { marginTop = 10 } };
        roomTemplateFoldout.Add(roomName);

        //room prefab Help Box
        HelpBox prefabTutorial = new HelpBox("Select your Room Prefab. This will be used later to create the dungeon layout", HelpBoxMessageType.Info){ style = { marginTop = 10 } };
        helpBoxes.Add(prefabTutorial);
        roomTemplateFoldout.Add(prefabTutorial);

        //room prefab
        ObjectField roomObjectField = new ObjectField();
        roomObjectField.objectType = typeof(GameObject);
        roomObjectField.label = "Room Prefab: ";
        roomObjectField.style.marginTop = 10;

        roomTemplateFoldout.Add(roomObjectField);

        roomObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) => {
            _roomPrefab = evt.newValue;
        });


        //helpbox room node type
        HelpBox roomNodeTutorial = new HelpBox("Select your room type, this will help use the correct node when creating the level node graph", HelpBoxMessageType.Info) { style = { marginTop = 10 } };
        helpBoxes.Add(roomNodeTutorial);
        roomTemplateFoldout.Add(roomNodeTutorial);

        //ROOM NODE TYPE LIST TO POPULATE POPUP
        RoomNodeTypeListSO roomList = GameResources.Instance.roomNodeTypeList;
        PopupField<RoomNodeTypeSO> popupField = new PopupField<RoomNodeTypeSO>("Room Node Type", roomList.list, 0, null, null) { style = { marginTop = 10 } };
        roomTemplateFoldout.Add(popupField);

        //BOUNDS
        HelpBox lowerBoundsTutorial = new HelpBox("If you imagine a rectangle around the room tilemap that just completely " +
            "encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined " +
            "from the tilemap for the room. Using the coordinate brush pointer to get the tilemap grid " +
            "position for that bottom left corner (Note: this is the local tilemap position and NOT world position)", HelpBoxMessageType.Info) { style = { marginTop = 10 } };
        helpBoxes.Add(lowerBoundsTutorial);
        roomTemplateFoldout.Add(lowerBoundsTutorial);

        Vector2IntField lowerBounds = new Vector2IntField("Lower Bounds") { style = { marginTop = 10 } };
        roomTemplateFoldout.Add(lowerBounds);

        HelpBox upperBoundsTutorial = new HelpBox("If you imagine a rectangle around the room tilemap that just completely " +
            "encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined " +
            "from the tilemap for the room. Using the coordinate brush pointer to get the tilemap grid position for that top " +
            "right corner (Note: this is the local tilemap position and NOT world position)", HelpBoxMessageType.Info) { style = { marginTop = 10 } };
        helpBoxes.Add(upperBoundsTutorial);
        roomTemplateFoldout.Add(upperBoundsTutorial);

        Vector2IntField upperBounds = new Vector2IntField("Upper Bounds") { style = { marginTop = 10 } };
        roomTemplateFoldout.Add(upperBounds);

        _scrollView.Add(roomTemplateFoldout);

        _foldout = roomTemplateFoldout;
    }

    private void TutorialToggle() {
        Toggle tutorialsToggle = new Toggle("Show Tutorial");
        tutorialsToggle.value = true;
        tutorialsToggle.SetEnabled(true);
        tutorialsToggle.AddToClassList("tutorial-toggle");

        tutorialsToggle.RegisterCallback<ChangeEvent<bool>>((evt) => {
            if (evt.newValue == true) {
                if (helpBoxes.Count > 0) {
                    foreach (var box in helpBoxes) {
                        box.style.display = DisplayStyle.Flex;
                    }
                }
            }
            else {
                if (helpBoxes.Count > 0) {
                    foreach (var box in helpBoxes) {
                        box.style.display = DisplayStyle.None;
                    }
                }
            }
        });
        _root.Add(tutorialsToggle);
    }

}




