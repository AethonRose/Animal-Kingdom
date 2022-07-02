using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    
    //Room uniquie identifier, GUID
    public string id;
    //List of parent GUIDS
    public List<string> roomNodeParentIDList = new List<string>();
    //List of child GUIDS
    public List<string> roomNodeChildIDList = new List<string>();
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

        //If roomNode has a parentNode or the roomNodeType isEntrance Set label for roomNode
        if (roomNodeParentIDList.Count > 0 || roomNodeType.isEntrance)
        {
            //Labels roomNode with roomNodeTypeName
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        
        //If roomNode has no Parents or isn't Entrance, Set roomNode to include the RoomNodeType Selection Popup
        else
        {
            //Look through RoomNodeTypes, FindIndex == roomNodeType
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            //Display Popup with GetRoomNodeTypesToDisplay and Default Nodes to the selected Type for the Node on next window open
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            //Set current roomNodeType to the selection index of roomNodeTypeList
            roomNodeType = roomNodeTypeList.list[selection];

            //Check for any changes in NodeTypeSelection in order to break Links when conditions met
            //
            //  [selected] isCorridor > [selection] is !Corridor 
            //  [selected] is !Corridor > [selection] isCorridor 
            //  [selected] is !BossRoom > [selection] isBossRoom
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || 
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                //If a roomNodeType has changed and has children, delete parentChild Links in order to revalidate
                if (roomNodeChildIDList.Count > 0)
                {
                    //Loop through each roomNodeChild in roomNode
                    for (int i = roomNodeChildIDList.Count - 1; i >= 0; i--)
                    {
                        //Get childRoomNode
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(roomNodeChildIDList[i]);

                        //If childRoomNode isValid
                        if (childRoomNode != null)
                        {
                            //Remove childID from Parent Room Node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            //Remove parentID from Child Room Node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(this.id);
                        }
                    }
                }
            }
        }
       
        
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
    public void DragNode(Vector2 delta)
    {
        //Move currentRoomNode position to the added movement since last Event
        rect.position += delta;
        //Notifies Unity about an update and saves
        EditorUtility.SetDirty(this);
    }

    //IsChildRoomValid - Called in AddChildRoomNodeIDToRoomNode - RoomNode Valid Checks
    public bool IsChildRoomValid(string childID)
    {

        bool isBossNodeAlreadyConnected = false;

        //Loop through each roomNode in roomNodeList
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            //If roomNodeType isBossRoom & parentIDList count > 0 Set BossNodeAlreadyConnected = true
            if (roomNode.roomNodeType.isBossRoom && roomNode.roomNodeParentIDList.Count > 0)
            {
                isBossNodeAlreadyConnected = true;
            }
        }

        //Allows for the creation of only 1 BossRoom if BossNodeAlreadyConnected
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isBossNodeAlreadyConnected)
        {
            return false;
        }

        //Disallows the connection of isNone roomNodes
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }

        //Disallows childing node multiple times to other roomNode If the roomNode already has a child with this childID return false, not valid
        if (roomNodeChildIDList.Contains(childID))
        {
            return false;
        }

        //Disallows childing of self If this nodeID == childID return false, not valid
        if (this.id == childID)
        {
            return false;
        }

        //Disallows parenting of self If this parentRoomNodeIDList contains this nodeID return false, not valid
        if (roomNodeParentIDList.Contains(this.id))
        {
            return false;
        }

        //Allow only 1 parent/line per roomNode
        if (roomNodeGraph.GetRoomNode(childID).roomNodeParentIDList.Count > 0)
        {
            return false;
        }

        //Disallows connections of corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }

        //Disallows rooms being next to eachother w/o a Corrider inbetween If both childID & roomNodeType are not of Type Corrider return false, not valid
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }

        //If child isCorridor & 3 corridors are already attached, 4 directions (except Entrance[3 directions]), return false not valid
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeChildIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }

        //Disallows the childing of an Entrance If childID is of Type Entrance, as Entrance should be top/root parent
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }

        //Disallows the chliding of more than 1 room If childRoomID isRoom and childList > 0
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeChildIDList.Count > 0)
        {
            return false;
        }

        //Return Valid/True on successful journey through checks
        return true;
  
    }

    //AddChildRoomNodeIDToRoomNode - Called in RoomNodeGraphEditor > ProcessMouseUpEvent - Sets childID and returns true
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            roomNodeChildIDList.Add(childID);
            return true;
        }

        return false;
        
    }

    //AddParentRoomNodeIDToRoomNode - Called in RoomNodeGraphEditor > ProcessMouseUpEvent - Sets parentID and returns true
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        roomNodeParentIDList.Add(parentID);
        return true;
    }

    //RemoveChildRoomNodeIDFromRoomNode - Called in RoomNodeGraphEditor.cs > DeleteSelectedRoomNodeLinks & DeleteSelectedRoomNodes - Remove childID from a roomNode
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {

        //If roomNodes childID list contains childID
        if (roomNodeChildIDList.Contains(childID))
        {
            //Remove childID from roomNodeChildIDList
            roomNodeChildIDList.Remove(childID);
            return true;
        }
        //Return false if roomNodeChildIDList !Contrain childID
        return false;

    }

    //RemoveParentRoomNodeIDFromRoomNode - Called in RoomNodeGraphEditor.cs > DeleteSelectedRoomNodeLinks & DeleteSelectedRoomNodes - Remove parentID from a roomNode
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {


        //If roomNodes ParentIDList Contains parentID, remove parentID from ParentIDList
        if (roomNodeParentIDList.Contains(parentID))
        {
            roomNodeParentIDList.Remove(parentID);
            return true;
        }
        //False if roomNodeParentIDList !Contain parentID
        return false;

    }

#endif

}
