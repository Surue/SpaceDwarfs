using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class SO_RuleTile : ScriptableObject {

    public abstract bool IsThisTile(MapTile tile, MapTile[,] mapTile);

}
