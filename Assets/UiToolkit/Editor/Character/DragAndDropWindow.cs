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
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UiToolkit/Editor/Character/DragAndDropWindow.uss");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        //MAIN TITLE
        Label windowTitle = new Label(" ~ CHARACTER CREATOR ~ ");
        windowTitle.AddToClassList("main-title");
        root.Add(windowTitle);

        //BIG SLOT
        _bigSlot = new VisualElement() { name = "bigSlot" };
        _bigSlot.AddToClassList("big_slot");
        root.Add(_bigSlot);
        _slots.Add(_bigSlot);

        InitSprites(root);


        /*
        VisualElement row1 = new VisualElement() { name = "row1" };
        row1.AddToClassList("slot_row");

        VisualElement slot1 = new VisualElement() { name = "slot1" };
        slot1.AddToClassList("slot");
        row1.Add(slot1);   
        
        VisualElement slot2 = new VisualElement() { name = "slot2" };
        slot2.AddToClassList("slot");
        row1.Add(slot2);

        root.Add(row1);

        _slots.Add(slot1);
        _slots.Add(slot2);

        StyleBackground backgroundImage = new StyleBackground(_humanSprite);
        Debug.Log(_clothes.name);
        VisualElement obj = new VisualElement() { name = "object", style = { backgroundImage = backgroundImage }};
        obj.AddToClassList("object");
        root.Add(obj);
        */


       //DragAndDropManipulator manipulator = new DragAndDropManipulator(obj, this);
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

            root.Add(newObject);

            //Place object at center of slot
            Vector2 targetCenter = new Vector2( newSlot.layout.x + newSlot.layout.width / 2f,
                                                newSlot.layout.y + newSlot.layout.height / 2f);

            newObject.transform.position = newSlot.transform.position;
            newObject.style.left = targetCenter.x - newObject.layout.width / 2f;
            newObject.style.top = targetCenter.y - newObject.layout.height / 2f;

            DragAndDropManipulator manipulator = new DragAndDropManipulator(newObject, this);
            i++;
        }
        root.Add(gridContainer);

    }
}