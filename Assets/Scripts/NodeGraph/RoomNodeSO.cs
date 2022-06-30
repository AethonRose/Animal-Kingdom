using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    
    //Room uniquie identifier, GUID
    public string id;
    //List of parent GUIDS
    public List<string> parentRoomNodeIDList = new List<string>();
    //List of child GUIDS
    public List<string> childRoomNodeIDList = new List<string>();
    //Init roomNodeGraph as type RoomNodeGraphSO
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    //Init roomNodetypeList as type RoomNodeTypeListSO
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    //Init roomNodeType as type RoomNodeTypeSO
    public RoomNodeTypeSO roomNodeType;

//Runs only if in UNITY_EDITOR
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    //Set when roomNode is being dragged
    [HideInInspector] public bool isLeftClickDragging = false;
    //Set when roomNode isSelected
    [HideInInspector] public bool isSelected = false;

    //Initialise - Called in RoomNodeGraphEditor.cs > CreateRoomNode Overloaded Method
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Room Node";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        //Load roomNodeTypeList from GameResource Instance into roomNodeTypeList
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {

        //BeginArea - Draw Node Box
        GUILayout.BeginArea(rect, nodeStyle);
        //Start region to Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();
        //Look through RoomNodeTypes, FindIndex == roomNodeType
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        //Display Popup with GetRoomNodeTypesToDisplay and Default Nodes to the selected Type for the Node on next window open
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        //Set current roomNodeType to the selection index of roomNodeTypeList
        roomNodeType = roomNodeTypeList.list[selection];
        //If change is detected between Begin and End ChangeCheck Set this to Dirty
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this); //Set Dirty signals that something has changed
        }

        //EndArea
        GUILayout.EndArea();

    }

    //GetRoomNodeTypesToDisplay - Called in Draw
    public string[] GetRoomNodeTypesToDisplay()
    {
        //Init empty roomArray with length of roomNodeTypeList
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        //Loop through roomNodeTypeList while roomNodeTypeList.list.Count < i
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            //If roomNodeType at current index is set to displayIn the NodeGraphEditor - add to roomArray
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }
    //ProcessEvents - Called in RoomNodeGraphEditor.cs > ProcessEvents - Processes Events related to the RoomNodeSO
    public void ProcessEvents(Event currentEvent)
    {

        //Switch on type of currentEvent
        switch(currentEvent.type)
        {

            //On MouseDown
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            //On MouseUp
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
                
            //On MouseDrag
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    //ProcessMouseDownEvent - Called in ProcessEvents - Processes Events related to MouseDown
    void ProcessMouseDownEvent(Event currentEvent)
    {
        //If Left Mouse Button Down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        //If Right Mouse Button Down
        if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    //ProcessMouseUpEvent - Called in ProcessEvents - Processes Events related to MouseUp
    void ProcessMouseUpEvent(Event currentEvent)
    {
        //If Left Mouse Button Up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }

    }


    //ProcessMouseDragEvent - Called in ProcessEvents - Processes Events related to MouseDrag
    void ProcessMouseDragEvent(Event currentEvent)
    {
        //If Left Mouse Button Drag
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    //ProcessLeftClickDownEvent - Called in ProcessMouseDownEvent - Processes Events related to LeftClickDown
    void ProcessLeftClickDownEvent()
    {
        //Sets activeObject in inspector as current Object selected; this.
        Selection.activeObject = this;

        //Toggle whether or not a roomNode isSelected
        isSelected = !isSelected;
    }

    //ProcessRightClickDownEvent - Called in ProcessMouseDownEvent - Processes Events related to RightClickDown
    void ProcessRightClickDownEvent(Event currentEvent)
    {
        //Set a Room Node to draw a line from on RightClick Event, passing in this roomNode and mousePosiiton
        roomNodeGraph.SetRoomNodeToDrawLineFrom(this, currentEvent.mousePosition);
    }

    //ProcessLeftClickUpEvent - Called in ProcessMouseUpEvent - Processes Events related to LeftClickUp
    void ProcessLeftClickUpEvent()
    {
        //If isLeftClick == true, set it to false on LeftClickUp
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    //ProcessLeftClickDragEvent - Called in ProcessMouseDragEvent - Processes Events related to LeftClickDragging
    void ProcessLeftClickDragEvent(Event currentEvent)
    {
        //Set isLeftClickDragging = true
        isLeftClickDragging = true;

        //Call DragNode and pass the currentEvent.delta, which gives the relative movement of mouse since last Event
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    //DragNode - Called in ProcessLeftClickDragEvent - Handles LeftClickDrag Movement of roomNodes
    void DragNode(Vector2 delta)
    {
        //Move currentRoomNode position to the added movement since last Event
        rect.position += delta;
        //Notifies Unity about an update and saves
        EditorUtility.SetDirty(this);
    }
    //AddChildRoomNodeIDToRoomNode - Called in RoomNodeGraphEditor > ProcessMouseUpEvent - Sets childID and returns true
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        childRoomNodeIDList.Add(childID);
        return true;
    }
    //AddParentRoomNodeIDToRoomNode - Called in RoomNodeGraphEditor > ProcessMouseUpEvent - Sets parentID and returns true
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }


#endif
}
