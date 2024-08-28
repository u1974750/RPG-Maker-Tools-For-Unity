using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;


public class ItemCreator : EditorWindow {

    private VisualElement root;
    private ScrollView scrollView;
    private HelpBox noSpriteHelpBox;
    private HelpBox noDurationHelpBox;
    private List<ToggleButtonGroup> buttonGroups = new List<ToggleButtonGroup>();
    private GroupBox timeGroupBox;
    private VisualElement bigSlot;
    private TextField itemName;
    private Sprite actualItemSprite;

    private int healthValue = 0;
    private int armourValue = 0;
    private int strenghtValue = 0;
    private int speedValue = 0;

    [MenuItem("Character Creator/Item Creator")]
    public static void OpwnItemWindow() {
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

        InitHelpBoxes();

        CreateBottomPart();
    }

    private void CreateTopPart() {

        //Main title
        Label windowTitle = new Label(" ~ ITEM CREATOR ~ ");
        windowTitle.AddToClassList("main-title");
        root.Add(windowTitle);

        //scroll view

        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.SetEnabled(true);
        root.Add(scrollView);

        //h2
        Label propertiesLabel = new Label("Item Properties");
        propertiesLabel.AddToClassList("h2-title");
        scrollView.Add(propertiesLabel);

        //Alignment box
        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("alignment-box");
        scrollView.Add(topBox);

        //Big Slot
        bigSlot = new VisualElement() { name = "bigSlot" };
        bigSlot.AddToClassList("big-slot");
        topBox.Add(bigSlot);

        //vertical alignment
        VisualElement columnBox = new VisualElement();
        columnBox.AddToClassList("column_box_alignment");

        //Item info
        itemName = new TextField(label: "• Item Name") { name = "ItemName" };
        columnBox.Add(itemName);

        //Item name Child
        VisualElement aux = itemName.Children().ToList()[0];
        aux.style.paddingLeft = 20f;

        //Item Sprite
        UnityEditor.UIElements.ObjectField itemSprite = new UnityEditor.UIElements.ObjectField() { name = "ItemSprite" };
        itemSprite.objectType = typeof(Sprite);
        itemSprite.label = "• Item Sprite";
        columnBox.Add(itemSprite);

        itemSprite.RegisterCallback<ChangeEvent<UnityEngine.Object>>((item) => {
            Sprite i = item.newValue as Sprite;
            StyleBackground backgroundImage = new StyleBackground(i);
            actualItemSprite = i;

            if (bigSlot.childCount != 0) {
                bigSlot.Clear();
            }

            VisualElement aux = new VisualElement() {
                style = {
                    backgroundImage = backgroundImage,
                    width = 100,
                    height = 100
                }
            };
            aux.AddToClassList("object");
            bigSlot.Add(aux);

        });

        //itemSprite child
        aux = itemSprite.Children().ToList()[0];
        aux.style.paddingLeft = -18f;
        aux.style.minWidth = 100f;

        //TimeDuration Label
        Label itemDurationLabel = new Label("• Item Duration") { name = "ItemDuration" };
        columnBox.Add(itemDurationLabel);

        //Duration Group Box
        timeGroupBox = new GroupBox();
        columnBox.Add(timeGroupBox);

        RadioButton timeButton1 = new RadioButton("30  seconds");
        timeButton1.value = true;
        timeGroupBox.Add(timeButton1);

        RadioButton timeButton2 = new RadioButton("1  minute");
        timeGroupBox.Add(timeButton2);

        RadioButton timeButton3 = new RadioButton("3  minutes");
        timeGroupBox.Add(timeButton3);
        topBox.Add(columnBox);

    }

    private void CreateBottomPart() {
        //container
        VisualElement containerBox = new VisualElement() { name = "ContainterBox" };
        scrollView.Add(containerBox);

        //h2
        Label propertiesLabel = new Label("Item Effects");
        propertiesLabel.AddToClassList("h2-title");
        containerBox.Add(propertiesLabel);

        //Buttons
        buttonGroups.Add(CreateButtonGroup(0, "Health", containerBox));
        buttonGroups.Add(CreateButtonGroup(1, "Armour", containerBox));
        buttonGroups.Add(CreateButtonGroup(2, "Strenght", containerBox));
        buttonGroups.Add(CreateButtonGroup(3, "Speed", containerBox));

        //Create Button
        Button createButton = new Button(CreateItem);
        createButton.text = "Create Item";
        createButton.style.marginTop = 30f;
        //createButton.SetEnabled(false);
        containerBox.Add(createButton);
    }

    private void InitHelpBoxes() {
        noSpriteHelpBox = new HelpBox("There is no sprite selected! Please select one", HelpBoxMessageType.Error);
        noSpriteHelpBox.style.display = DisplayStyle.None;
        root.Add(noSpriteHelpBox);

        noDurationHelpBox = new HelpBox("There is no duration time selected! Please select one", HelpBoxMessageType.Error);
        noDurationHelpBox.style.display = DisplayStyle.None;
        root.Add(noDurationHelpBox);
    }

    private ToggleButtonGroup CreateButtonGroup(int i, string name, VisualElement container) {
        string symbol = "";

        switch (i) {
            case 0: symbol = "\u2665"; break; //health
            case 1: symbol = "\U0001F6E1"; break; //armour
            case 2: symbol = "\u2694"; break; //strength
            case 3: symbol = "\U0001F97E"; break; //speed
        }

        Button labelButton = new Button();
        labelButton.text = symbol + " " + name;
        labelButton.AddToClassList("label-button");
        labelButton.clicked += () => SetActiveButton(i, 0);

        ToggleButtonGroup toggleGroup = new ToggleButtonGroup();


        toggleGroup.Add(CreateStatButton(i, "-3", -3));
        toggleGroup.Add(CreateStatButton(i, "-2", -2));
        toggleGroup.Add(CreateStatButton(i, "-1", -1));
        toggleGroup.Add(labelButton);
        toggleGroup.Add(CreateStatButton(i, "+1", 1));
        toggleGroup.Add(CreateStatButton(i, "+2", 2));
        toggleGroup.Add(CreateStatButton(i, "+3", 3));

        container.Add(toggleGroup);

        return toggleGroup;
    }

    private Button CreateStatButton(int i, string txt, int val) {
        float buttonWidth = 45f;

        Button aux = new Button() { text = txt, style = { width = buttonWidth } };
        aux.clicked += () => SetActiveButton(i, val);

        return aux;
    }

    private void SetActiveButton(int category, int val) {
        if (val < -3) val = -3;
        if (val > 3) val = 3;

        switch (category) {
            case 0: healthValue = val; break;
            case 1: armourValue = val; break;
            case 2: strenghtValue = val; break;
            case 3: speedValue = val; break;
            default: break;
        }
    }

    private bool CanCreateItem() {
        bool hasSprite = true;
        bool hasTime = true;

        //check if has sprite
        if (bigSlot.childCount == 0) {
            noSpriteHelpBox.style.display = DisplayStyle.Flex;
            hasSprite = false;
        }
        else {
            noSpriteHelpBox.style.display = DisplayStyle.None;
            hasSprite = true;

        }

        //check if has time
        RadioButton timeButton1 = timeGroupBox.Children().ToList()[0] as RadioButton;
        RadioButton timeButton2 = timeGroupBox.Children().ToList()[1] as RadioButton;
        RadioButton timeButton3 = timeGroupBox.Children().ToList()[2] as RadioButton;

        if (!timeButton1.value && !timeButton2.value && !timeButton3.value) {
            noDurationHelpBox.style.display = DisplayStyle.Flex;
            hasTime = false;
        }
        else {
            noDurationHelpBox.style.display = DisplayStyle.None;
            hasTime = true;
        }

        return hasTime && hasSprite;
    }

    private void CreateItem() {

        if (CanCreateItem()) {

            GameObject newItem = new GameObject();

            Item itemScript = newItem.AddComponent<Item>();

            string name = "My new Item";
            if (itemName.value != "") { name = itemName.value; }

            itemScript.SetItemName(name);
            itemScript.SetItemSprite(actualItemSprite);

            float time = 0;
            for (int i = 0; i < 3; i++) {
                RadioButton timeButton = timeGroupBox.Children().ToList()[i] as RadioButton;
                if (timeButton.value) {
                    if (i == 0) { time = 30f; break; }
                    else if (i == 1) { time = 60f; break; }
                    else if (i == 2) { time = 180f; break; }
                }
            }
            itemScript.SetItemTime(time);
            itemScript.SetItemValues(healthValue, armourValue, strenghtValue, speedValue);



            //Save as a prefab

            string path = "Assets/Prefabs/Items/" + name + ".prefab";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            GameObject newGameObject = PrefabUtility.SaveAsPrefabAsset(newItem, path);
            AssetDatabase.SaveAssets();

            //Destroy temporary GameObject
            DestroyImmediate(newItem);

            //ping the new character
            EditorGUIUtility.PingObject(newGameObject);
        }

    }


}
