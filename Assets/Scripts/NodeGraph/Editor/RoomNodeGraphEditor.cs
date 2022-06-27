using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; //Needed for EditorWindow

//Creating Dungeon Editor Window in Unity

public class RoomNodeGraphEditor : EditorWindow //Replacing MonoBehavior with EditorWindow
{
    GUIStyle roomNodeStyle;

    const float nodeWidth = 100f;
    const float nodeHeight = 75f;
    const int nodePadding = 25;
    const int nodeBorder = 12;

    //Make new line in Window tab in unity, set the name of new window option & set path of MenuItem
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/ Room Node Graph Editor")]

//Ask Nick why OpenWindow runs without being called, does it have anything to do with being static?
    //Runs when window opens
    static void OpenWindow() //MenuItem requires static
    {
        //Starts window of type RoomNodeGraphEditor & set window title
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    void OnEnable()
    {
        SetRoomNodeStyle();
    }

    void OnGUI() 
    {
        CreateRoomNode(100f,100f,"Node 1");
        CreateRoomNode(100f,200f, "Node 2");
    }

    // Set Style of roomNodeStyle
    void SetRoomNodeStyle()
    {
        //Set roomNodeStyle background, textColor, padding & border
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }
    
    // Creates node on RoomNodeGraphEditor Window
    void CreateRoomNode(float xNodePosition, float yNodePosition, string nodeLabel)
    {
        //BeginArea begins a GUILayout block of GUI controls in a fixed screen area
        GUILayout.BeginArea(new Rect(new Vector2(xNodePosition,yNodePosition), new Vector2(nodeWidth,nodeHeight)), roomNodeStyle);
        //Label node
        EditorGUILayout.LabelField(nodeLabel);
        //Closes GUILayout block to stop any GUIClip errors
        GUILayout.EndArea();
    }
}
