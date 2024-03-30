using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class RoomBuilder : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement root;

    [MenuItem("Dungeon Builder/Room Builder")]
    public static void OpenRoomBuilderWindow()
    {
        RoomBuilder wnd = GetWindow<RoomBuilder>();
        wnd.titleContent = new GUIContent("RoomBuilder");
    }


    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        


    }

    private void OnGUI() {
        GetImageForSprite();
    }

    private void GetImageForSprite() {
        Debug.Log("Hola");
        if(root != null) {
            Object inputPrefab = root.Q<ObjectField>("PrefabObjectField").value;
            if (inputPrefab != null) {
               // AssetPreview.SetPreviewTextureCacheSize(200);
                Texture2D previewTexture = AssetPreview.GetAssetPreview(inputPrefab);
                VisualElement roomPreviewImage = root.Q<VisualElement>("RoomSprite");
                roomPreviewImage.style.backgroundImage = previewTexture;
            }          
        }        
    }

}




