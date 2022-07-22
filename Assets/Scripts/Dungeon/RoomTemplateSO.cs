using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
   [HideInInspector] public string guid;

   #region Header ROOM PREFAB

   [Space(10)]
   [Header("ROOM PREFAB")]

   #endregion Header ROOM PREFAB
   #region Tooltip

   [Tooltip("The GameObject prefab for the room (this will contain all the tilefamps for the room and environment gameObjects)")]
   
   #endregion Tooltip
   //previoud Prefab saved in order to regen guid if the SO is copied and the prefab is changed
   [HideInInspector] public GameObject previousPrefab;
   public GameObject prefab;



   #region Header ROOM CONFIG
   
   [Space(10)]
   [Header("ROOM CONFIG")]
   
   #endregion Header ROOM CONFIG
   #region Tooltip
   
   [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph.")]
   
   #endregion Tooltip
   public RoomNodeTypeSO roomNodeType;

   #region Tooltip

   [Tooltip("Bottom left bounds of room")]
   
   #endregion ToolTip
    public Vector2Int lowerBounds;
   
   #region Tooltip

   [Tooltip("Upper right bounds of room")]
   
   #endregion ToolTip
   public Vector2Int upperBounds;

   #region Tooltip

   [Tooltip("Max of 4 doorways per room, with a 3 tile width, with the middle tile being the doorway coordinate 'position'")]

   #endregion Tooltip
   [SerializeField] public List<Doorway> doorwayList;

   #region Tooltip

   [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coords should be added to this array")]
   
   #endregion Tooltip
   public Vector2Int[] spawnPositionArray;

   //GetDoorwayList - Returns back doorwayList list
   public List<Doorway> GetDoorwayList()
   {

    return doorwayList;

   }

    #region Validation

#if UNITY_EDITOR

    //Validate SO fields
    private void OnValidate() 
    {
        //Set GUID if empty of the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
        //Check spawn positions populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);

    }

#endif

    #endregion Validation


}
