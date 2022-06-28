using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    
    //Room uniquie identifier, GUID
    [HideInInspector] public string id;
    //List of parent GUIDS
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    //List of child GUIDS
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    //Init roomNodeGraph as type RoomNodeGraphSO
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    //Init roomNodeType as type RoomNodeTypeSO
    public RoomNodeTypeSO roomNodeType;
    //Init roomNodetypeList as type RoomNodeTypeListSO
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

//Runs only if in UNITY_EDITOR
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

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

#endif



}
