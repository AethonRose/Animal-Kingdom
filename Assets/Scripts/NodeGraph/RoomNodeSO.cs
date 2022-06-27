using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    
    //Room uniquie identifier, GUID
    [HideInInspector] public string roomNodeID;

    //List of parent GUIDS
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();

    //List of child GUIDS
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();

    //RoomNodeGraphSO reference
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;

}
