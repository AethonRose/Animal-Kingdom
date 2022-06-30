using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CreateAssetMenu allows the Scriptable Object to be auto listed in the Assets/Create submenu, so instances can be easily created.
[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{

    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    void Awake() 
    {
        LoadRoomNodeDictionary();
    }

    //LoadRoomNodeDictionary - Called on Awake - Clears then populates roomNodeDictionary with each roomNode in roomNodeList
    void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        //Loop through each roomNode in roomNodeList and add it to roomNodeDictionary
        foreach(RoomNodeSO roomNode in roomNodeList)
        {
            roomNodeDictionary[roomNode.id] = roomNode;
        }
    }

#if UNITY_EDITOR

    //Node to draw line from
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    //Node to draw line to
    [HideInInspector] public Vector2 linePosition;

    //OnValidate - Called in RoomNodeGraphEditor > CreateRoomNode - Calls everytime script loads or has update
    public void OnValidate() 
    {
        LoadRoomNodeDictionary();    
    }

    public void SetRoomNodeToDrawLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

}
