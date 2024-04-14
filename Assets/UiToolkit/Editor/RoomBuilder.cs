using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using NG.Elements;
using System.Linq;

public class RoomBuilder : EditorWindow
{
    
    //private VisualTreeAsset _VisualTreeAsset = default;
    private VisualElement _root;
    private List<HelpBox> helpBoxes = new List<HelpBox>();
    private ScrollView _scrollView;
    private Foldout _foldout;
    private UnityEngine.Object _roomPrefab;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private Texture2D _imagePlaceholder;
    [SerializeField] private Texture2D _doorSprite;

    [MenuItem("Dungeon Builder/Room Builder")]
    public static void OpenRoomBuilderWindow()
    {        
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

        //FOLDOUT 1: ROOM PREFAB
        CreateRoomPrefabFoldout();

        // FOLDOUT 2: ROOM TEMPLATE S.O.
        RoomTemplateFoldout();

        //CREATE SCRIPTABLE OBJECT
        Button createObjectButton = new Button(CreateAssetButtonAction) { text = "Create Room" };
        _root.Add(createObjectButton);

    }

    private void CreateAssetButtonAction()
    {
        List<VisualElement> childrenList = _foldout.Children().ToList();

        RoomTemplateSO newRoom = ScriptableObject.CreateInstance<RoomTemplateSO>();
        TextField inputName = (TextField)childrenList[0];
        string path = "Assets/ScriptableObjectAssets/Dungeon/Rooms/" + inputName.value + ".asset"; // change object name!

        newRoom.prefab = (GameObject)_roomPrefab;

        var roomTypeInput = (PopupField<RoomNodeTypeSO>)childrenList[2];
        newRoom.roomNodeType = roomTypeInput.value;

        Vector2IntField lowerBoundInput = (Vector2IntField) childrenList[4];
        newRoom.lowerBounds = lowerBoundInput.value;

        Vector2IntField upperBoundInput = (Vector2IntField) childrenList[6];
        newRoom.upperBounds = upperBoundInput.value;


        List<VisualElement> doorwayTabs = childrenList[8].Children().ToList()[0].Children().ToList();
        newRoom.doorwayList = new List<Doorway>();

        for(int i = 0; i < 4; i++) {
            List<VisualElement> doorWay = doorwayTabs[i].Children().ToList();
            Doorway newDoorway = new Doorway();

            EnumField doorOrientation = (EnumField)doorWay[0];
            newDoorway.orientation = (RoomOrientation)doorOrientation.value;
            
            Vector2IntField doorPos = (Vector2IntField)doorWay[1];
            newDoorway.position = doorPos.value;

            Vector2IntField startCopyPos = (Vector2IntField)doorWay[3];
            newDoorway.doorwayStartCopyPosition = startCopyPos.value;

            IntegerField tileWidth = (IntegerField)doorWay[4];
            newDoorway.doorwayCopyTileWidth = tileWidth.value;

            IntegerField tileHeight = (IntegerField)doorWay[5];
            newDoorway.doorwayCopyTileHeight = tileHeight.value;

            newRoom.doorwayList.Add(newDoorway);
        }

        
        AssetDatabase.CreateAsset(newRoom, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRoom;
        

    }

    private void Doorways(Foldout foldout) {
        HelpBox doorHelper = new HelpBox("There should be a maximum of four doorways for a room" +
            " - one for each compass direction.  These should have a consistent 3 tile opening size," +
            " with the middle tile position being the doorway coordinate 'position'", HelpBoxMessageType.Info);
        helpBoxes.Add(doorHelper);
        foldout.Add(doorHelper);

        var box = new Box();
        box.AddToClassList("doorways-box");
               
        var DoorwaysTab = new TabView() { style = { marginTop = 15} }; 

        for(int i = 0; i < 4;  i++) {
            var door = new Tab("Door " + (i+1), _doorSprite);

            EnumField doorOrientation = new EnumField("Door Orientation", RoomOrientation.none) { style = { marginTop = 10 } };
            door.Add(doorOrientation);

            Vector2IntField doorPos = new Vector2IntField("Door Position") { style = { marginTop = 10 } };
            door.Add(doorPos);

            HelpBox copyPositionHelp = new HelpBox("The Upper Left Position To Start Copying From", HelpBoxMessageType.Info);
            helpBoxes.Add(copyPositionHelp);
            door.Add(copyPositionHelp);

            Vector2IntField startCopyField = new Vector2IntField("Start Copy Position") { style = { marginTop = 10 } };
            door.Add(startCopyField);

            IntegerField copyWidth = new IntegerField("Copy Tiles Width") { style = { marginTop = 10 } };
            door.Add(copyWidth);

            IntegerField copyHeight = new IntegerField("Copy Tiles Height") { style = { marginTop = 10 } };
            door.Add(copyHeight);

            DoorwaysTab.Add(door);

        }
        
        box.Add(DoorwaysTab);
        foldout.Add(box);

    }

    //2. TOGGLE FOR THE ROOM TEMPLATE S.O. DETAILS
    private void RoomTemplateFoldout() {
        Foldout roomTemplateFoldout = new Foldout { text = "2. Room Template" };

        TextField roomName = new TextField() { label = "Room Name: "};
        roomTemplateFoldout.Add(roomName);

        //helpbox
        HelpBox roomNodeTutorial = new HelpBox("Select your room type, this will help use the correct node when creating the level node graph", HelpBoxMessageType.Info);
        helpBoxes.Add(roomNodeTutorial);
        roomTemplateFoldout.Add(roomNodeTutorial);

        //ROOM NODE TYPE LIST TO POPULATE POPUP
        RoomNodeTypeListSO roomList = GameResources.Instance.roomNodeTypeList;
        PopupField<RoomNodeTypeSO> popupField = new PopupField<RoomNodeTypeSO>("Room Node Type", roomList.list, 0, null, null);
        roomTemplateFoldout.Add(popupField);
        //TODO: Canviar noms de les variables al editor per tal de ser mes clars

        //BOUNDS
        HelpBox lowerBoundsTutorial = new HelpBox("If you imagine a rectangle around the room tilemap that just completely " +
            "encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined " +
            "from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid " +
            "position for that bottom left corner (Note: this is the local tilemap position and NOT world position\"", HelpBoxMessageType.Info);
        helpBoxes.Add(lowerBoundsTutorial);
        roomTemplateFoldout.Add(lowerBoundsTutorial);

        Vector2IntField lowerBounds = new Vector2IntField("Lower Bounds");
        roomTemplateFoldout.Add(lowerBounds);

        HelpBox upperBoundsTutorial = new HelpBox("If you imagine a rectangle around the room tilemap that just completely " +
            "encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined " +
            "from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top " +
            "right corner (Note: this is the local tilemap position and NOT world position", HelpBoxMessageType.Info);
        helpBoxes.Add(upperBoundsTutorial);
        roomTemplateFoldout.Add(upperBoundsTutorial);

        Vector2IntField upperBounds = new Vector2IntField("Upper Bounds");
        roomTemplateFoldout.Add(upperBounds);

        Doorways(roomTemplateFoldout);

        _scrollView.Add(roomTemplateFoldout);
        _foldout = roomTemplateFoldout;
    }

    //1. TOGGLE FOR THE ROOM PREFAB
    private void CreateRoomPrefabFoldout() {
        Foldout roomPrefabFoldout = new Foldout { text = "1. Create Room Prefab" };
        HelpBox prefabTutorial = new HelpBox("Select your Room Prefab to visualize it. You can skip this part, " +
                                    "it's only to help you visualize the room", HelpBoxMessageType.Info);
        helpBoxes.Add(prefabTutorial);
        roomPrefabFoldout.Add(prefabTutorial);

        ObjectField roomObjectField = new ObjectField();
        roomObjectField.objectType = typeof(GameObject);
        roomObjectField.label = "Room Prefab: ";

        roomPrefabFoldout.Add(roomObjectField);

        Image roomPreview = new Image();
        //roomPreview.image = _imagePlaceholder;
        roomPreview.AddToClassList("room-placeholder-image");
        //roomPreview.style.backgroundImage = Resources.Load<Texture2D>("Resources/placeholder-image.png");
        roomPrefabFoldout.Add(roomPreview);

        roomObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) => {
            roomPreview.image = AssetPreview.GetAssetPreview(evt.newValue);
            roomPreview.RemoveFromClassList("room-placeholder-image");
            roomPreview.AddToClassList("room-prefab-image");
            _roomPrefab = evt.newValue;
        });

        _scrollView.Add(roomPrefabFoldout);
    }

    private void TutorialToggle() {
        Toggle tutorialsToggle = new Toggle("Show Tutorial");
        tutorialsToggle.value = true;
        tutorialsToggle.SetEnabled(true);
        tutorialsToggle.AddToClassList("tutorial-toggle");

        tutorialsToggle.RegisterCallback<ChangeEvent<bool>>((evt) => {
            if (evt.newValue == true) {
                if(helpBoxes.Count > 0) {
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




