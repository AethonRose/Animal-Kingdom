using UnityEngine;
[System.Serializable]

public class Doorway
{
    //if the doorway isConnected, it will be Available
    [HideInInspector] public bool isConnected = false;
    //if the door way is !Connected, it will be Unavailable
    [HideInInspector] public bool isUnavailable = false;

    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;

    #region Header

    [Header("Upper Left Position to Start Copying From")]

    #endregion
    public Vector2Int doorwayStartCopyPosition;

    #region Header

    [Header("Width of the tiles in the doorway to copy over")]

    #endregion
    public int doorwayCopyTileWidth;

    #region Header

    [Header("Height of the tiles in the doorway to copy over")]
    
    #endregion
    public int doorwayCopyTileHeight;

}
