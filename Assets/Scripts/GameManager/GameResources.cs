using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    static GameResources instance;

    //Creates asset of requested type to be called on later
    public static GameResources Instance
    {

        //Runs when an instance of GameResources is requested
        get
        {

            //If instance of type GameResources == null load  GameResources
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            //If !null return instance
            return instance;
        }
    }

    #region DUNGEON
    [Header("Dungeon")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with RoomNodeTyleListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;
}
