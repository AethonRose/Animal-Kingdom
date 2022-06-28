
using UnityEngine;
using UnityEditor; //EditorWindow
using UnityEditor.Callbacks; //OnOpenAsset
using System;

public class RoomNodeGraphEditor : EditorWindow //Replacing MonoBehavior with EditorWindow
{
    GUIStyle roomNodeStyle; //Style info for GUI Elements
    static RoomNodeGraphSO currentRoomNodeGraph; //currentRoomNodeGraph reference
    RoomNodeSO currentRoomNode = null;
    RoomNodeTypeListSO roomNodeTypeList; //roomNodeTypeList reference

    //roomNodeStyle Values
    const float nodeWidth = 200f;
    const float nodeHeight = 75f;
    const int nodePadding = 25;
    const int nodeBorder = 12;

    //MenuItem - Make new line in Window tab in unity, set the name of new window option & set path of MenuItem
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/ Room Node Graph Editor")]
    //OpenWindow - Called on MenuItem - Opens window of class RoomNodeGraphEditor
    static void OpenWindow() //MenuItem requires static
    {
        //Starts window of type RoomNodeGraphEditor & set window title
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    //OnEnable - Called when object is loaded - Sets roomNodeStyle and loads roomNodeTypes
    void OnEnable()
    {
        //Set roomNodeStyles
        SetRoomNodeStyle();

        //Load room node type from GameResources prefab
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    //SetRoomNodeStyle - Called in OnEnable - Creates style for roomNode
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
    //OnDoubleClickAsset - Called on OnOpenAsset - Open roomNodeGraph window that is double clicked
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

    //ProcessEvents - Called in OnGUI - Processes any Input related Events
    void ProcessEvents(Event currentEvent)
    {

        //Execute if currentRoomNode == null || isn't being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            //Set currentRoomNode to one being Moused Over, return null if no Node
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        //Only Execute if currentRoomNode == null, Stop Execute when roomNode is being hovered
        if (currentRoomNode == null)
        {
            //Process Events on RoomNodeGraph
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        //Only Executes when currentRoomNode != null or when a roomNode is being moused over
        else
        {
            //Process currentRoomNode Events
            currentRoomNode.ProcessEvents(currentEvent);
        }
        
    }

    //IsMouseOverRoomNode - Called in ProcessEvents - Returns roomNode that Mouse is Over
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        //Loop through the currentRoomNodeGraph roomNodeList
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            //Get roomNode in roomNodeList and if its rect Contains currentEvent.mousePosition then return that roomNode 
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }

    //ProcessRoomNodeGraphEvents - Called in ProcessEvents - Procceses currentEvents related to RoomNodeGraph
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

    //ProcessMouseDownEvent - Called in ProcessRoomNodeGraphEvents - Processes currentEvents related to MouseDown
    void ProcessMouseDownEvent(Event currentEvent)
    {
        //If right mouse button is pressed Call ShowContextMenu at the currentEvents mousePosition
        if (currentEvent.button == 1) //1 = right mouse, 0 = left mouse button
        {
            //Bring up right click Context Menu
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    //ShowContextMenu - Called in ProcessMouseDownEvent - Creates menu Items, assigning functions to each item and displaying
    void ShowContextMenu(Vector2 mousePosition)
    {
        //Set menu as datatype GenericMenu 
        GenericMenu menu = new GenericMenu(); //datatype GenericMenu custom context and dropdown windows
        //AddItem to rightclick menu, set title, currently active on/off , function to call when item is selected, and current mousePosition
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        //Shows menu under mouse when right clicked
        menu.ShowAsContext();
    }

    //CreateRoomNode - Called in ShowContextMenu > menu.AddItem - Creates RoomNodes
    void CreateRoomNode(object mousePositionObject)
    {
        //Calls overloaded CreateRoomNode Method passing mousePositionObject, and the RoomNodeType of isNone for a default state
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone)); //predicate
    }

    //CrateRoomNode Overload - Called in CreateRoomNode - Creates RoomNodes
    void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        //Set mousePosition
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //Set roomNode = CreateInstance of RoomNode Scriptable Object
        RoomNodeSO roomNode = CreateInstance<RoomNodeSO>();

        //Add roomNode to the currentRoomNodeGraph roomNodeList
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        //Initialise roomNode
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth,nodeHeight)), currentRoomNodeGraph, roomNodeType);
        
        //Add roomNode asset to be childed under currentRoomNodeGraph
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph); //assetdatabase is an interface for accessing and performing operations on assets
        
        //Writing all unsaved asset changes to disk
        AssetDatabase.SaveAssets();

    }
    //DrawRoomNodes - Called in OnGUI - Draw all roomNodes in roomNodeList
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
}
