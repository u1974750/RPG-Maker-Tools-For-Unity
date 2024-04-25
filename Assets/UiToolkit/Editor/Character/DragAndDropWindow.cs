using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class DragAndDropWindow : EditorWindow {

    private List<VisualElement> _slots = new List<VisualElement>();

    public List<VisualElement> Slots => _slots;

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

        Label windowTitle = new Label(" ~ CHARACTER CREATOR ~ ");
        windowTitle.AddToClassList("main-title");
        root.Add(windowTitle);


        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UiToolkit/Editor/Character/DragAndDropWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
       // root.Add(labelFromUXML);

        VisualElement bigSlot = new VisualElement() { name = "bigSlot" };
        bigSlot.AddToClassList("big_slot");
        root.Add(bigSlot);
        _slots.Add(bigSlot);

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

        VisualElement obj = new VisualElement() { name = "object"};
        obj.AddToClassList("object");
        root.Add(obj);


       DragAndDropManipulator manipulator = new DragAndDropManipulator(obj, this);
    }
}