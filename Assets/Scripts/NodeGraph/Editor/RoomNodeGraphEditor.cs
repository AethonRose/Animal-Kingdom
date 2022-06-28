
using UnityEngine;
using UnityEditor; //EditorWindow
using UnityEditor.Callbacks; //OnOpenAsset
using System;

public class RoomNodeGraphEditor : EditorWindow //Replacing MonoBehavior with EditorWindow
{
    GUIStyle roomNodeStyle; //Style info for GUI Elements
    static RoomNodeGraphSO currentRoomNodeGraph; //currentRoomNodeGraph reference
    RoomNodeTypeListSO roomNodeTypeList; //roomNodeTypeList reference

    //roomNodeStyle Values
    const float nodeWidth = 200f;
    const float nodeHeight = 75f;
    const int nodePadding = 25;
    const int nodeBorder = 12;

    //MenuItem - Make new line in Window tab in unity, set the name of new window option & set path of MenuItem
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/ Room Node Graph Editor")]
    //OpenWindow - Called on MenuItem
    static void OpenWindow() //MenuItem requires static
    {
        //Starts window of type RoomNodeGraphEditor & set window title
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    //Called when object is loaded
    void OnEnable()
    {
        //Set roomNodeStyles
        SetRoomNodeStyle();

        //Load room node type from GameResources prefab
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    //SetRoomNodeStyle - Called in OnEnable
    void SetRoomNodeStyle()
    {
        //Set roomNodeStyle background, textColor, padding & border
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }

    //OnOpenAsset - called when double clicking an asset in the Project Browser
    [OnOpenAsset(0)] // Number basically same a z-order, 1 will call after 0 and so on
    //OnDoubleClickAsset - Called on OnOpenAsset
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        //Set roomNodeGraph to and object of instanceID and save it as the same datatype; RoomNodeGraphSO
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        //!Null check for roomNodeGraph, calling OpenWindow, setting currentRoomNodeGraph
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }

    //OnGUI - Called everytime GUI has update event
    void OnGUI() 
    {
        //!Null check on currentRoomNodeGraph
        if (currentRoomNodeGraph != null)
        {
            //Process Mouse & Keyboard events
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        //Redraw UI if GUI has changed
        if (GUI.changed)
        {
            Repaint();
        }
    }

    //ProcessEvents - Called in OnGUI
    void ProcessEvents(Event currentEvent)
    {
        //Process Events on RoomNodeGraph
        ProcessRoomNodeGraphEvents(currentEvent);
    }

    //ProcessRoomNodeGraphEvents - Called in ProcessEvents
    void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        //Switchs on currentEvent type (KeyUp/Down, MouseUp/Down, etc.)
        switch (currentEvent.type)
        {
            //Process MouseDown Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    //ProcessMouseDownEvent - Called in ProcessRoomNodeGraphEvents
    void ProcessMouseDownEvent(Event currentEvent)
    {
        //If right mouse button is pressed Call ShowContextMenu at the currentEvents mousePosition
        if (currentEvent.button == 1) //1 = right mouse, 0 = left mouse button
        {
            //Bring up right click Context Menu
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    //ShowContextMenu - Called in ProcessMouseDownEvent
    void ShowContextMenu(Vector2 mousePosition)
    {
        //Set menu as datatype GenericMenu 
        GenericMenu menu = new GenericMenu(); //datatype GenericMenu custom context and dropdown windows
        //AddItem to rightclick menu, set title, currently active on/off , function to call when item is selected, and current mousePosition
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        //Shows menu under mouse when right clicked
        menu.ShowAsContext();
    }

    //CreateRoomNode - Called in ShowContextMenu > menu.AddItem
    void CreateRoomNode(object mousePositionObject)
    {
        //Calls overloaded CreateRoomNode Method passing mousePositionObject, and the RoomNodeType of isNone
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone)); //predicate
    }

    //CrateRoomNode Overload - Called in CreateRoomNode
    void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        //Set mousePosition
        Vector2 mousePosition = (Vector2)mousePositionObject;
        //Set roomNode to a CreatedInstance of a RoomNodeSO
        RoomNodeSO roomNode = CreateInstance<RoomNodeSO>();
        //Add roomNodeSO item to currentRoomNodeGraph's roomNodeList
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        //Initialist roomNode
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth,nodeHeight)), currentRoomNodeGraph, roomNodeType);
        //Adds roomNode to the currentRoomNodeGraph Scriptable Object
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph); //assetdatabase is an interface for accessing and performing operations on assets
        //Writing all unsaved asset changes to disk
        AssetDatabase.SaveAssets();

    }
    //DrawRoomNodes - Called in OnGUI
    void DrawRoomNodes()
    {
        //Loop through each room node in currentRoomNodeGraph.roomNodeList and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }
        //GUI has changed with cause a repaint in OnGUI
        GUI.changed = true;
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
