using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

public class DragAndDropWindow : EditorWindow {

    private List<VisualElement> _slots = new List<VisualElement>();
    private VisualElement _bigSlot;
    [SerializeField] Sprite _character;
    [SerializeField] Sprite _clothes;
    private Sprite _humanSprite;
    private bool isNPC = true;
    private bool isMelee = true;
    private bool isPattrol = true;
    private HelpBox _spriteHelpBox;

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
        //_slots.Add(_bigSlot);

        //PROPERTIES
        VisualElement columnBox = new VisualElement();
        columnBox.AddToClassList("column_box_alignment");

        ToggleButtonGroup toggleController = new ToggleButtonGroup("Controller");
        toggleController.Add(new Button(setNpcButton) { text = "NPC", tooltip = "The character will be controlled by AI" });
        toggleController.Add(new Button(setPlayerButton) { text = "Player", tooltip = "The character will be controlled by the player" });
        columnBox.Add(toggleController);

        ToggleButtonGroup toggleStats = new ToggleButtonGroup("CharacterStats");
        toggleStats.Add(new Button(setMeleeButton) { text = "Melee Attacker", tooltip = "The character will attack with short weapons" });
        toggleStats.Add(new Button(setRangeButton) { text = "Range Attacker", tooltip = "The character will attack from a distance" });
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
        InitSprites(tabOne);
    }


    private void CreateCharacterButton() { 

        if (_bigSlot.childCount == 0 ) {
            _spriteHelpBox.style.display = DisplayStyle.Flex;
        }
        else {
            _spriteHelpBox.style.display = DisplayStyle.None;
            string spriteName = _bigSlot.Children().ToList()[0].name;

            //Create Game Object
            GameObject newCharacter = new GameObject();

            //Sprite Renderer
            SpriteRenderer spriteRenderer = newCharacter.AddComponent<SpriteRenderer>();
            Sprite sprite = Resources.Load<Sprite>("Characters_Sprite/" + spriteName);
            if(sprite != null) { spriteRenderer.sprite = sprite; }

            //Animator Controller
            Animator animator = newCharacter.AddComponent<Animator>();
            string removedName = spriteName.Substring(4);
            string newName = "animator_" + removedName;
            UnityEditor.Animations.AnimatorController animController = Resources.Load<UnityEditor.Animations.AnimatorController>("Animators/" + newName);
            animator.runtimeAnimatorController = animController;

            //collider
            newCharacter.AddComponent<CapsuleCollider2D>();

            //Script
            if (isNPC) { newCharacter.AddComponent<EnemyController>(); }
            else { newCharacter.AddComponent<PlayerMovement>(); }


            //Save as a prefab
            string path = "Assets/Prefabs/Characters/" + _bigSlot.Children().ToList()[0].name + ".prefab";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            GameObject newGameObject = PrefabUtility.SaveAsPrefabAsset(newCharacter,path);
            AssetDatabase.SaveAssets();

            //Destroy temporary GameObject
            GameObject.DestroyImmediate(newCharacter);
            
            //ping the new character
            EditorGUIUtility.PingObject(newGameObject);

        }

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
            _slots.Add(newSlot);
            gridContainer.Add(newSlot);

            //CREATE OBJECT
            StyleBackground backgroundImage = new StyleBackground(obj);
            VisualElement newObject = new VisualElement() { name = obj.name,
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

    private void setNpcButton() { isNPC = true; }
    private void setPlayerButton() { isNPC = false; }
    private void setMeleeButton() { isMelee = true; }
    private void setRangeButton() { isMelee = false; }
    private void setPattrolButton() { isPattrol = true; }
    private void setStaticButton() { isPattrol = false; }

}