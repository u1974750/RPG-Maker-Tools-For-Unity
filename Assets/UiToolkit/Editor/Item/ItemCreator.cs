using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemCreator : EditorWindow
{

    private VisualElement root;
    private int healthValue;

    [MenuItem("Character Creator/Item Creator")]
    public static void OpwnItemWindow()
    {
        ItemCreator wnd = GetWindow<ItemCreator>();
        wnd.titleContent = new GUIContent("ItemCreator");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
                        ("Assets/UiToolkit/Editor/Item/ItemCreator.uss");
        if (styleSheet != null) root.styleSheets.Add(styleSheet);

        CreateTopPart();
        CreateBottomPart();
    }

    private void CreateTopPart() {

        //Main title
        Label windowTitle = new Label(" ~ ITEM CREATOR ~ ");
        windowTitle.AddToClassList("main-title");
        root.Add(windowTitle);

        //h2
        Label propertiesLabel = new Label("Item Properties");
        propertiesLabel.AddToClassList("h2-title");
        root.Add(propertiesLabel);

        //Alignment box
        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("alignment-box");
        root.Add(topBox);

        //Big Slot
        VisualElement bigSlot = new VisualElement() { name = "bigSlot" };
        bigSlot.AddToClassList("big-slot");
        topBox.Add(bigSlot);

        //vertical alignment
        VisualElement columnBox = new VisualElement();
        columnBox.AddToClassList("column_box_alignment");

        //Item info
        TextField itemName = new TextField(label: "• Item Name") { name = "ItemName"};
        columnBox.Add(itemName);

        //Item name Child
        VisualElement aux = itemName.Children().ToList()[0];
        aux.style.paddingLeft = 20f;

        //Item Sprite
        ObjectField itemSprite = new ObjectField() { name = "ItemSprite"};
        itemSprite.objectType = typeof(Sprite);
        itemSprite.label = "• Item Sprite";
        columnBox.Add(itemSprite);
        
        //itemSprite child
        aux = itemSprite.Children().ToList()[0];
        aux.style.paddingLeft = -18f;
        aux.style.minWidth = 100f;

        //TimeDuration Label
        Label itemDurationLabel = new Label("• Item Duration") { name = "ItemDuration" };
        columnBox.Add(itemDurationLabel);

        //Duration Group Box
        GroupBox timeGroupBox = new GroupBox();
        columnBox.Add(timeGroupBox);

        RadioButton timeButton1 = new RadioButton("30  seconds");
        timeGroupBox.Add(timeButton1);
        
        RadioButton timeButton2 = new RadioButton("1  minute");
        timeGroupBox.Add(timeButton2);

        RadioButton timeButton3 = new RadioButton("3  minutes");
        timeGroupBox.Add(timeButton3);

        topBox.Add(columnBox);

    }

    private void CreateBottomPart() {
        //container
        VisualElement containerBox = new VisualElement() { name = "ContainterBox"};
        root.Add(containerBox);

        //h2
        Label propertiesLabel = new Label("Item Effects");
        propertiesLabel.AddToClassList("h2-title");
        containerBox.Add(propertiesLabel);

        //Buttons
        CreateButtonGroup(0, "Health",   containerBox);
        CreateButtonGroup(1, "Armour",   containerBox);
        CreateButtonGroup(2, "Strenght", containerBox);
        CreateButtonGroup(3, "Speed",    containerBox);

        //Create Button
        Button createButton = new Button();
        createButton.text = "Create Item";
        createButton.style.marginTop = 50f;
        //createButton.SetEnabled(false);
        containerBox.Add(createButton);
    }

    private void CreateButtonGroup(int i, string name, VisualElement container) {
        string symbol = "";
        float buttonWidth = 35f;
        switch (i) {
            case 0: symbol = "\u2665";     break; //health
            case 1: symbol = "\U0001F6E1"; break; //armour
            case 2: symbol = "\u2694";     break; //strength
            case 3: symbol = "\U0001F97E"; break; //speed
        }

        Button labelButton = new Button();
        labelButton.text = symbol + " " + name;
        labelButton.style.width = 80f;
        labelButton.SetEnabled(false);

        ToggleButtonGroup toggleGroupHealth = new ToggleButtonGroup();
        toggleGroupHealth.Add(new Button() { text = "-3", style = { width = buttonWidth}});
        toggleGroupHealth.Add(new Button() { text = "-2", style = { width = buttonWidth } });
        toggleGroupHealth.Add(new Button() { text = "-1", style = { width = buttonWidth } });
        toggleGroupHealth.Add(labelButton);
        toggleGroupHealth.Add(new Button() { text = "+1", style = { width = buttonWidth } });
        toggleGroupHealth.Add(new Button() { text = "+2", style = { width = buttonWidth } });
        toggleGroupHealth.Add(new Button() { text = "+3", style = { width = buttonWidth } });

        container.Add(toggleGroupHealth);
    }
}
