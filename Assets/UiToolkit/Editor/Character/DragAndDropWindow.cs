using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

public class DragAndDropWindow : EditorWindow {

    private List<VisualElement> _slots = new List<VisualElement>();
    private VisualElement _bigSlotFirstTab;
    private VisualElement _bigSlotSecondTab;
    [SerializeField] Sprite _character;
    [SerializeField] Sprite _clothes;
    private Sprite _humanSprite;
    private bool _isNPC = true;
    private bool _isMelee = true;
    private bool _isPattrol = true;
    private HelpBox _spriteHelpBox;


    public List<VisualElement> Slots => _slots;
    public VisualElement BigSlot => _bigSlotFirstTab;
    public VisualElement BigSlotSecondTab => _bigSlotSecondTab;

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

        Tab tabOne = new Tab("Premade Sprites");
        mainTabView.Add(tabOne);

        Tab tabTwo = new Tab("Sprite Creator");
        mainTabView.Add(tabTwo);

        CreateTabOneContent(tabOne);
        CreateTabTwoContent(tabTwo);

        root.Add(mainTabView);
    }

    private void CreateCharacterButton() {

        if (_bigSlotFirstTab.childCount == 0) {
            _spriteHelpBox.style.display = DisplayStyle.Flex;
        }
        else {
            _spriteHelpBox.style.display = DisplayStyle.None;
            string spriteName = _bigSlotFirstTab.Children().ToList()[0].name;

            //Create Game Object
            GameObject newCharacter = new GameObject();

            //Sprite Renderer
            SpriteRenderer spriteRenderer = newCharacter.AddComponent<SpriteRenderer>();
            Sprite sprite = Resources.Load<Sprite>("Characters_Sprite/" + spriteName);
            if (sprite != null) { spriteRenderer.sprite = sprite; }
            spriteRenderer.sortingLayerName = "Player";

            //Animator Controller
            Animator animator = newCharacter.AddComponent<Animator>();
            string removedName = spriteName.Substring(4);
            string newName = "animator_" + removedName;
            UnityEditor.Animations.AnimatorController animController = Resources.Load<UnityEditor.Animations.AnimatorController>("Animators/" + newName);
            animator.runtimeAnimatorController = animController;

            //collider
            newCharacter.AddComponent<CapsuleCollider2D>();

            //Script
            if (_isNPC) {
                EnemyController controller = newCharacter.AddComponent<EnemyController>();
                controller.isMelee = _isMelee;
                controller.isPattrol = _isPattrol;
            }
            else { newCharacter.AddComponent<PlayerMovement>(); }


            //Save as a prefab
            string path = "Assets/Prefabs/Characters/" + removedName + ".prefab";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            GameObject newGameObject = PrefabUtility.SaveAsPrefabAsset(newCharacter, path);
            AssetDatabase.SaveAssets();

            //Destroy temporary GameObject
            GameObject.DestroyImmediate(newCharacter);

            //ping the new character
            EditorGUIUtility.PingObject(newGameObject);

        }

    }

    private void CreateTabOneContent(Tab tabOne) {
        //VISUAL ELEMENT BIG BOX
        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("alignment-box");
        tabOne.Add(topBox);

        //BIG SLOT
        _bigSlotFirstTab = new VisualElement() { name = "bigSlot" };
        _bigSlotFirstTab.AddToClassList("big_slot");
        topBox.Add(_bigSlotFirstTab);

        //PROPERTIES
        VisualElement columnBox = new VisualElement();
        columnBox.AddToClassList("column_box_alignment");

        ToggleButtonGroup toggleController = new ToggleButtonGroup("Controller");
        toggleController.Add(new Button(setNpcButton) { text = "NPC", tooltip = "The character will be controlled by AI" });
        toggleController.Add(new Button(setPlayerButton) { text = "Player", tooltip = "The character will be controlled by the player" });
        columnBox.Add(toggleController);

        ToggleButtonGroup toggleStats = new ToggleButtonGroup("Attach Mode");
        toggleStats.Add(new Button(setMeleeButton) { text = "Melee", tooltip = "The character will attack with short weapons" });
        toggleStats.Add(new Button(setRangeButton) { text = "Range", tooltip = "The character will attack from a distance" });
        columnBox.Add(toggleStats);

        ToggleButtonGroup togglePattrol = new ToggleButtonGroup("Pattrol");
        togglePattrol.Add(new Button(setPattrolButton) { text = "Pattrol", tooltip = "The character will pattrol through the map points" });
        togglePattrol.Add(new Button(setStaticButton) { text = "Static", tooltip = "The character will be still at the spawn point" });
        columnBox.Add(togglePattrol);

        Button createButton = new Button(CreateCharacterButton);
        createButton.text = "Create Character";
        columnBox.Add(createButton);


        topBox.Add(columnBox);

        _spriteHelpBox = new HelpBox("There is no Sprite selected! You must select one", HelpBoxMessageType.Error);
        _spriteHelpBox.style.display = DisplayStyle.None;
        tabOne.Add(_spriteHelpBox);

        //SPRITES
        InitSprites(tabOne, 1);
    }

    private void CreateTabTwoContent(Tab tabTwo) {
        //VISUAL ELEMENT BIG BOX
        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("alignment-box");
        tabTwo.Add(topBox);

        //BIG SLOT
        _bigSlotSecondTab = new VisualElement() { name = "bigSlot2" };
        _bigSlotSecondTab.AddToClassList("big_slot");
        topBox.Add(_bigSlotSecondTab);

        //SIDE BUTTONS
        VisualElement columnBox = new VisualElement();
        columnBox.AddToClassList("column_box_alignment");

        Button removeArmourButton = new Button(RemoveArmourAction);
        removeArmourButton.text = "Remove Armour";
        columnBox.Add(removeArmourButton);

        Button removeClothesButton = new Button(RemoveClothesAction);
        removeClothesButton.text = "Remove Clothes";
        columnBox.Add(removeClothesButton);

        Button removeHairButton = new Button(RemoveHairAction);
        removeHairButton.text = "Remove Hair";
        columnBox.Add(removeHairButton);

        Button removeHatButton = new Button(RemoveHatAction);
        removeHatButton.text = "Remove Hat";
        columnBox.Add(removeHatButton);

        Button removeAllButton = new Button(ClearSkeletonAction);
        removeAllButton.text = "Clear";
        columnBox.Add(removeAllButton);

        topBox.Add(columnBox);

        //SPRITE SLOTS
        InitSprites(tabTwo, 2);

        Button createSkeleton = new Button(createCustomCharacter);
        createSkeleton.text = "Create Character";
        tabTwo.Add(createSkeleton);
    }
    private VisualElement createSlot(string name) {
        VisualElement slot = new VisualElement() { name = name };
        slot.AddToClassList("slot");
        return slot;
    }

    private void InitSprites(VisualElement root, int tab) {
        Object[] allSprites = new Object[0];
        if (tab == 1) {
            allSprites = Resources.LoadAll("Characters_Sprite", typeof(Sprite));
        }
        else if (tab == 2) {
            allSprites = Resources.LoadAll("Character_Creator", typeof(Sprite));
        }
        int i = 1;

        VisualElement gridContainer = new VisualElement() { name = "row1" };
        gridContainer.style.width = new StyleLength(Length.Percent(100));
        gridContainer.AddToClassList("slot_row");

        foreach (Sprite obj in allSprites) {
            //CREATE OBJECT
            StyleBackground backgroundImage = new StyleBackground(obj);
            VisualElement newObject = new VisualElement() {
                name = obj.name,
                style = {
                    backgroundImage = backgroundImage
                }
            };
            newObject.AddToClassList("object");

            if (tab == 2 && obj.name == "CCreator_skeleton") {
                newObject.style.height = 100;
                newObject.style.width = 100;
                _bigSlotSecondTab.Add(newObject);
            }
            else {
                VisualElement newSlot = createSlot("Slot" + i);
                _slots.Add(newSlot);
                gridContainer.Add(newSlot);
                newSlot.Add(newObject);
                DragAndDropManipulator manipulator = new DragAndDropManipulator(newObject, this, root);
            }

            i++;
        }
        root.Add(gridContainer);

    }

    #region SetCharacterBehaviour
    private void setNpcButton() { _isNPC = true; }
    private void setPlayerButton() { _isNPC = false; }
    private void setMeleeButton() { _isMelee = true; }
    private void setRangeButton() { _isMelee = false; }
    private void setPattrolButton() { _isPattrol = true; }
    private void setStaticButton() { _isPattrol = false; }
    #endregion

    #region RemoveSkeletonParts
    private void RemoveArmourAction() {
        RemoveAction("Armour");
    }
    private void RemoveClothesAction() {
        RemoveAction("Clothes");
    }
    private void RemoveHairAction() {
        RemoveAction("Hair");
    }
    private void RemoveHatAction() {
        RemoveAction("Hat");
    }
    private void ClearSkeletonAction() {
        ClearAllSkeleton();
    }
    private void RemoveAction(string toRemove) {
        List<VisualElement> childObject = BigSlotSecondTab.Children().ToList();

        for (int i = 0; i < childObject.Count; i++) {

            string[] childNames = childObject[i].name.Split("_");

            if (childNames[childNames.Length - 1] == toRemove) {
                childObject[i].RemoveFromHierarchy();
                for (int j = 0; j < Slots.Count; j++) {
                    if (Slots[j].childCount == 0) {
                        VisualElement target;
                        if (Slots[j + 1] != null) { target = Slots[j + 1]; }
                        else { target = Slots[j]; }
                        Slots[j].Add(childObject[i]);
                        childObject[i].style.height = target.style.height;
                        childObject[i].style.width = target.style.width;
                        childObject[i].transform.position = Slots[i].transform.position;
                    }
                }
            }
        }
    }
    private void ClearAllSkeleton() {

        List<VisualElement> childObject = BigSlotSecondTab.Children().ToList();
        for (int i = 1; i < childObject.Count; i++) {
            childObject[i].RemoveFromHierarchy();
            for (int j = 0; j < Slots.Count; j++) {
                if (Slots[j].childCount == 0) {
                    VisualElement target;
                    if (Slots[j + 1] != null) { target = Slots[j + 1]; }
                    else { target = Slots[j]; }
                    Slots[j].Add(childObject[i]);
                    childObject[i].style.height = target.style.height;
                    childObject[i].style.width = target.style.width;
                    childObject[i].transform.position = Slots[i].transform.position;
                }
            }
        }
    }

    #endregion

    private void createCustomCharacter() {
        Debug.Log("TODO: unir les parts del personatge ");
    }
}