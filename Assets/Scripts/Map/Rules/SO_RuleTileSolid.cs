using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CreateAssetMenu(fileName = "ruleTile_name", menuName = "Tilemape / Rule Tile Solid")]
[System.Serializable]
public class Rule_TileSolid:SO_RuleTile {

    public Tile tile;

    public List<Rule> rules = new List<Rule>();

    [System.Serializable]
    public class Rule{
        public bool[] array = new bool[9];
    }
    
    //public bool[] rules = new bool[9];

    public override bool IsThisTile(MapTile tile, MapTile[,] mapTile) {
        if(!tile.isSolid) {
            return false; //TO CHANGE
        }

        foreach(Rule r in rules) {
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
            int width = mapTile.GetLength(0);
            int height = mapTile.GetLength(1);

            bool isThisRule = true;

            foreach(Vector3Int b in bounds.allPositionsWithin) {
                if(tile.position.x + b.x >= 0 && tile.position.x + b.x < width && tile.position.y + b.y >= 0 && tile.position.y + b.y < height) { //Is in the map
                    MapTile currentTile = mapTile[tile.position.x + b.x, tile.position.y + b.y];
                    int index = ((b.y + 1) * 3 ) + b.x + 1;
                    if(currentTile.isSolid && !r.array[index] || !currentTile.isSolid && r.array[index]) {
                        isThisRule = false;
                    }
                }
            }

            if(isThisRule) {
                return true;
            }
        }

        return false;
    }
}

//[CustomEditor(typeof(MapTile_so))]
//public class LevelEditor:Editor {

//    public Object source;

//    public bool showMap = true;

//    public override void OnInspectorGUI() {
//        MapTile_so levels = (MapTile_so)target;

//        levels.tile = EditorGUILayout.ObjectField("Tile associated : ", levels.tile, typeof(Object), false) as Tile;

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Set of rules for center tiles");
//        EditorGUILayout.LabelField("(Toggle on means it's a solid tile)");
//        EditorGUILayout.BeginHorizontal();

//        levels.rules[0] = EditorGUILayout.Toggle(levels.rules[0], GUILayout.Width(20));
//        levels.rules[1] = EditorGUILayout.Toggle(levels.rules[1], GUILayout.Width(20));
//        levels.rules[2] = EditorGUILayout.Toggle(levels.rules[2], GUILayout.Width(20));

//        EditorGUILayout.EndHorizontal();
//        EditorGUILayout.BeginHorizontal();

//        levels.rules[3] = EditorGUILayout.Toggle(levels.rules[3], GUILayout.Width(20));
//        levels.rules[4] = EditorGUILayout.Toggle(levels.rules[4], GUILayout.Width(20));
//        levels.rules[5] = EditorGUILayout.Toggle(levels.rules[5], GUILayout.Width(20));

//        EditorGUILayout.EndHorizontal();
//        EditorGUILayout.BeginHorizontal();

//        levels.rules[6] = EditorGUILayout.Toggle(levels.rules[6], GUILayout.Width(20));
//        levels.rules[7] = EditorGUILayout.Toggle(levels.rules[7], GUILayout.Width(20));
//        levels.rules[8] = EditorGUILayout.Toggle(levels.rules[8], GUILayout.Width(20));

//        EditorGUILayout.EndHorizontal();
//    }
//}