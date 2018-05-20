using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ruleTile_name", menuName = "Tilemap / Ore")]
[System.Serializable]
public class SO_Ore : ScriptableObject {

    [SerializeField]
    public Tile oreTile;

    [SerializeField]
    [Range(0, 1000)]
    public int oreScore;

    [SerializeField]
    [Range(0, 100)]
    public int apparitionRate;
}
