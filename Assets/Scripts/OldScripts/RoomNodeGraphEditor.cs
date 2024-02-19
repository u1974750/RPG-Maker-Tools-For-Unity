using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    //Node Layput Values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private GUIStyle _roomNodeStyle;

    [MenuItem("Room Node Graph Editor", menuItem = "Custom Tools/Dungeon Editor/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        //Define node layout style
        _roomNodeStyle = new GUIStyle();
        _roomNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        _roomNodeStyle.normal.textColor = Color.black;
        _roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        _roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

    }

    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(new Vector2(100f,100f), new Vector2(nodeWidth,nodeHeight)), _roomNodeStyle);
        EditorGUILayout.LabelField("NODE 1");
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(new Vector2(300f,300f), new Vector2(nodeWidth,nodeHeight)), _roomNodeStyle);
        EditorGUILayout.LabelField("NODE 2");
        GUILayout.EndArea();
    }
}
