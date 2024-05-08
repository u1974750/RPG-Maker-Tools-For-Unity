using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI.Table;

public class DragAndDropWindow : EditorWindow {

    private List<VisualElement> _slots = new List<VisualElement>();
    private VisualElement _bigSlot;
    [SerializeField] Sprite _character;
    [SerializeField] Sprite _clothes;
    private Sprite _humanSprite;
    public List<VisualElement> Slots => _slots;
    public VisualElement BigSlot => _bigSlot;

    [MenuItem("Character Creator/Drag And Drop")]
    public static void ShowExample() {
        DragAndDropWindow wnd = GetWindow<DragAndDropWindow>();
        wnd.titleContent = new GUIContent("Drag And Drop");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
                         ("Assets/UiToolkit/Editor/Character/DragAndDropWindow.uss");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        //MAIN TITLE
        Label windowTitle = new Label(" ~ CHARACTER CREATOR ~ ");
        windowTitle.AddToClassList("main-title");
        root.Add(windowTitle);

        //TOGGLE
        // Create a TabView with Tabs that only contains a label.
        TabView mainTabView = new TabView() { style = { marginTop = 15, marginLeft = 15 } };

        Tab tabOne = new Tab("One");
        mainTabView.Add(tabOne);

        Tab tabTwo = new Tab("Two");
        mainTabView.Add(tabTwo);

        root.Add(mainTabView);

        //VISUAL ELEMENT BIG BOX
        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("alignment-box");
        tabOne.Add(topBox);

        //BIG SLOT
        _bigSlot = new VisualElement() { name = "bigSlot" };
        _bigSlot.AddToClassList("big_slot");
        topBox.Add(_bigSlot);
        _slots.Add(_bigSlot);

        //PROPERTIES
        VisualElement columnBox = new VisualElement();

        ToggleButtonGroup ButtonGroup = new ToggleButtonGroup("Controller");
        ButtonGroup.Add(new Button() { text = "NPC", tooltip = "The character will be controlled by AI" });
        ButtonGroup.Add(new Button() { text = "Player", tooltip = "The character will be controlled by the player" });
        columnBox.Add(ButtonGroup);

        ToggleButtonGroup ButtonGroup1 = new ToggleButtonGroup("CharacterStats");
        ButtonGroup1.Add(new Button() { text = "Melee Attacker", tooltip = "The character will attack with short weapons" });
        ButtonGroup1.Add(new Button() { text = "Range Attacker", tooltip = "The character will attack from a distance" });
        columnBox.Add(ButtonGroup1);

        ToggleButtonGroup ButtonGroup2 = new ToggleButtonGroup("Pattrol");
        ButtonGroup2.Add(new Button() { text = "Pattrol", tooltip = "The character will pattrol through the map points" });
        ButtonGroup2.Add(new Button() { text = "Static", tooltip = "The character will be still at the spawn point" });
        columnBox.Add(ButtonGroup2);

        Button createButton = new Button();
        createButton.text = "Create Character";
        columnBox.Add(createButton);

        topBox.Add(columnBox);

        //SPRITES
        InitSprites(tabOne);
    }

    private VisualElement createSlot(string name) {
        VisualElement slot = new VisualElement() { name = name };
        slot.AddToClassList("slot");
        return slot;
    }

    private void InitSprites(VisualElement root) {

        Object[] allSprites = Resources.LoadAll("Characters_Sprite", typeof(Sprite));
        int i = 1;

        VisualElement gridContainer = new VisualElement() { name = "row1" };
        gridContainer.style.width = new StyleLength(Length.Percent(100)); 
        gridContainer.AddToClassList("slot_row");

        foreach (Sprite obj in allSprites) {
            VisualElement newSlot = createSlot( "Slot" + i );
            gridContainer.Add(newSlot);

            //CREATE OBJECT
            StyleBackground backgroundImage = new StyleBackground(obj);
            VisualElement newObject = new VisualElement() { name = "object" + i,
                style = {
                    backgroundImage = backgroundImage
                }
            };
            newObject.AddToClassList("object");
            newSlot.Add(newObject);

            DragAndDropManipulator manipulator = new DragAndDropManipulator(newObject, this, root);
            i++;
        }
        root.Add(gridContainer);

    }
}