using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Tooltip
    [Tooltip("List should be populated with all RoomNodeTypeSO - this is used inplace of enums")]
    #endregion Tooltip
    //Init list of datatype RoomNodeTypeSO;
    public List<RoomNodeTypeSO> list;

#if UNITY_EDITOR
    
    void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this,nameof(list),list);
    }

#endif

}
