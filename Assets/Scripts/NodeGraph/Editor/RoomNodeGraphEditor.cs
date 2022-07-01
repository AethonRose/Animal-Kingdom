using UnityEngine;
using UnityEditor; //EditorWindow
using UnityEditor.Callbacks; //OnOpenAsset
using System;

public class RoomNodeGraphEditor : EditorWindow //Replacing MonoBehavior with EditorWindow
{
    GUIStyle roomNodeStyle; //Style info for GUI Elements
    GUIStyle roomNodeSelectedStyle;
    
    static RoomNodeGraphSO currentRoomNodeGraph; //currentRoomNodeGraph reference
    RoomNodeSO currentRoomNode = null;
    RoomNodeTypeListSO roomNodeTypeList; //roomNodeTypeList reference

    //roomNodeStyle Values
    const float nodeWidth = 200f;
    const float nodeHeight = 75f;
    
    const int nodePadding = 25;
    const int nodeBorder = 12;
   
    const float connectingLineWidth = 3f;
    const float connectingLineArrowSize = 6f;

    //MenuItem - Make new line in Window tab in unity, set the name of new window option & set path of MenuItem
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/ Room Node Graph Editor")]

    //OpenWindow - Called on MenuItem - Opens window of class RoomNodeGraphEditor
    static void OpenWindow() //MenuItem requires static
    {

        //Starts window of type RoomNodeGraphEditor & set window title
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");

    }

    //OnEnable - Called when object is loaded
    void OnEnable()
    {
        
        //Runs InspectorSelectionChanged when current active item has changed, specifying the Inspector
        Selection.selectionChanged += InspectorSelectionChanged;

        //Set roomNodeStyles
        SetRoomNodeStyle();

        //Load room node type from GameResources prefab
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

    }

    //OnDisabled - Calls when object is unloaded
    void OnDisable() 
    {
        //Runs -InspectorSelectionChanged when current active item has changed, to unselect the Disabled Object 
        Selection.selectionChanged -= InspectorSelectionChanged;
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
            //Want to be drawn first, see if it can go below ProcessEvents
            DrawDraggedLine();
            //Process Mouse & Keyboard events
            ProcessEvents(Event.current);
            DrawRoomLines();
            //Process the drawing of  all roomNodes in roomNodeList
            DrawRoomNodes();
        }

        //Redraw UI if GUI has changed
        if (GUI.changed)
        {
            Repaint();
        }

    }

    //InspectorSelectionChanged - Called when Unity detects a change in InspectorSelection and in OnEnable & Disable - Changes currentRoomNodeGraph to one Selected in Inspector
    void InspectorSelectionChanged()
    {

        //Sets roomNodeGraph to current active Selected Object and saves the activeObject as a RoomNodeGraphSO
        RoomNodeGraphSO activeRoomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        //If roomNodeGraph is valid, set currentRoomNodeGraph = activeRoomNodeGraph
        if (activeRoomNodeGraph != null)
        {
            //Set currentRoomNodeGraph to one currently Selected in Inspector
            currentRoomNodeGraph = activeRoomNodeGraph;
            GUI.changed = true;
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

        //Only Execute if currentRoomNode == null or currently dragging a line from the currentRoomNode, Stop Execute when roomNode is being hovered
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
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
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
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

        //If LeftMouseDown ClearDraggedLine & ClearAllSelectedRoomNodes
        if (currentEvent.button == 0)
        {
            ClearDraggedLine();
            ClearAllSelectedRoomNodes();
        }

    }

    //ProcessMouseUpEvent - Calling in ProcessRoomNodeGraphEvents - Processes currentEvents related to MouseUp
    void ProcessMouseUpEvent(Event currentEvent)
    {

        //Executes if rightclick event and roomNodeToDrawLine from is valid
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            //returns null if mouse is not over a RoomNode, returns name of RoomNode if !null
            RoomNodeSO roomNodeHovered = IsMouseOverRoomNode(currentEvent);
            
            //If the roomNode being hovered is valid
            if (roomNodeHovered != null)
            {
                //If child ID can be set it will set the childID and then set parentID
                if(currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNodeHovered.id))
                {
                    //Set Parent ID in the child room node
                    roomNodeHovered.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            //Clears line on MouseUp
            ClearDraggedLine();
        }

    }

    //ProcessMouseDragEvent - Called in ProcessMouseDownEvent - Processes currentEvents related to MouseDragging
    void ProcessMouseDragEvent(Event currentEvent)
    {

        //If RightClickDrag Event
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }

    }

    //ProcessRightMouseDragEvent - Called in ProcessMouseDragEvent - Processes curentEvents related to RightMouseDragging
    void ProcessRightMouseDragEvent(Event currentEvent)
    {

        //Execute If the roomNodeToDrawLineFrom isn't null while rightclicking
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            SetCurrentLinePosition(currentEvent.delta);
            //set a gui change
            GUI.changed = true;
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
    
    //SetRoomNodeStyle - Called in OnEnable - Creates style for roomNode
    void SetRoomNodeStyle()
    {

        //Set Default roomNodeStyle background, textColor, padding & border
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Set Selected Node Style
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

    }

    //ShowContextMenu - Called in ProcessMouseDownEvent - Creates menu Items, assigning functions to each item and displaying
    void ShowContextMenu(Vector2 mousePosition)
    {

        //Set menu as datatype GenericMenu 
        GenericMenu menu = new GenericMenu(); //datatype GenericMenu custom context and dropdown windows

        //AddItem to rightclick menu, set title, currently active on/off , function to call when item is selected, and current mousePosition
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
       
        //Adds Select All RoomNodes button to ContextMenu
        menu.AddItem(new GUIContent("Select All RoomNodes"), false, SelectAllRoomNodes);
        
        //Shows menu under mouse when right clicked
        menu.ShowAsContext();

    }

    //CreateRoomNode - Called in ShowContextMenu > menu.AddItem - Creates RoomNodes
    void CreateRoomNode(object mousePositionObject)
    {
        
        //If no other roomNodes have been created create an Entrance Node
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            //Creates Entrance
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

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

        //Update CurrentRoomNodeGraphs RoomNodeDictionary
        currentRoomNodeGraph.OnValidate();

    }

    //SelectAllRoomNodes - Called in ShowContextMenu - 
    void SelectAllRoomNodes()
    {

        //Loops through each roomNode in Graphs roomNodeList
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            //Sets each roomNode to isSelected
            roomNode.isSelected = true;
        }

        GUI.changed = true;

    }

    //SetConnectingLinePosition - Called in ProcessRightMouseDragEvent -  Sets linePosition to mouse location
    void SetCurrentLinePosition(Vector2 delta)
    {

        //Set linePosition to where the mouse was moved last
        currentRoomNodeGraph.linePosition += delta;

    }

    void DrawDraggedLine()
    {

        //Executes if targetNodePosition is != 0
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {

            //Start & End positions of Line
            Vector2 lineStartPosition = currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center;
            Vector2 lineEndPosition = currentRoomNodeGraph.linePosition;

            //Draws line from currentNode to linePosition
            Handles.DrawBezier(lineStartPosition, lineEndPosition, lineStartPosition, lineEndPosition, Color.white, null, connectingLineWidth);
        }

    }

    //ClearLineDrag - Called in ProcessMouseUpEvent - Resets Line Drag setting roomNodeToDrawLineFrom & linePosition to null, zero
    void ClearDraggedLine()
    {

        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;

    }
    
    //DrawRoomNodes - Called in OnGUI - Draw all roomNodes in roomNodeList
    void DrawRoomNodes()
    {

        //Loop through each room node in currentRoomNodeGraph.roomNodeList and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            //If roomNode isSelected Draw roomNodeSelectedStyle, else Draw default roomNodeStyle
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        //GUI has changed with cause a repaint in OnGUI
        GUI.changed = true;

    }

    //DrawRoomLines - Called in OnGUI - Draw's lines from parentRoomNode to each childRoomNode, if roomNode contains any children
    void DrawRoomLines()
    {

        //Loops through each roomNode in currentRoomNodeGraphs roomNodeList, Drawing lines from parentRoomNode to each childRoomNode
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            //If roomNode has any child IDs
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                //Loops through each childRoomNodeID in roomNode's childRoomNodeIDList
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    //Draw lines from parentRoomNode to all childRoomNodeID's
                    DrawLineConnection(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                    GUI.changed = true;
                }
            }
        }

    }

    //DrawLineConnection - Called in DrawRoomConnections - Draws Bezier Line from parentNode to childNode
    void DrawLineConnection(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {

        //Sets Start and End Position of Parent to Child Lines
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        //Calculate midPosition of Line
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        //Calculate direction Line is going from Start to End
        Vector2 direction = endPosition - startPosition;

        //Calculate arrowTails by +ing and -ing mid position from perpendicular directions *ing by connectingLineArrowSize
        Vector2 arrowTailPoint1 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize; 
        Vector2 arrowTailPoint2 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        //Calculate mid point offset position for arrowHead
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        //Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        //Draws Line, replaced tangents with position to make straight line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;

    }

    //ClearAllSelectedRoomNodes - Called in ProcessMouseDownEvent - Clears selected roomNodes to set back to default Style
    void ClearAllSelectedRoomNodes()
    {

        //Loops through each roomNode in roomNodeList
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {

            //If roomNode is selected, Set isSelected to false to clear Selected roomNodes
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }

    }
    
}
