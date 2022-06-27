using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//CreateAssetMenu allows the Scriptable Object to be auto listed in the Assets/Create submenu, so instances can be easily created.
[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{

    public string roomNodeTypeName;

    #region Comment
    [Header("Only Show RoomNodeTypes that should be visible in the editor")]
    #endregion Comment
    public bool displayInNodeGraphEditor = true;

    #region Comment
    [Header("Room Type: Corridor")]
    #endregion Comment
    public bool isCorridor;

    #region Comment
    [Header("Room Type: CorridorNS")]
    #endregion Comment
    public bool isCorridorNS;

    #region Comment
    [Header("Room Type: CorridorEW")]
    #endregion Comment
    public bool isCorridorEW;

    #region Comment
    [Header("Room Type: Entrance")]
    #endregion Comment
    public bool isEntrance;

    #region Comment
    [Header("Room Type: BossRoom")]
    #endregion Comment
    public bool isBossRoom;

    #region Comment
    [Header("Room Type: None")]
    #endregion Comment
    public bool isNone;

   //Code will only run in UNITY_EDITOR
   #if UNITY_EDITOR
    //OnValidate called when the script is loaded or a value changes in the inspector
    void OnValidate() 
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
   #endif 
}
