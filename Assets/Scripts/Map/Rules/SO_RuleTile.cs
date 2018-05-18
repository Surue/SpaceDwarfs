using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CreateAssetMenu(fileName = "ruleTile_name", menuName = "Tilemap / Rule Tile Test")]
[System.Serializable]
public class SO_RuleTile : ScriptableObject {
    [SerializeField]
    public Tile tile;

    [SerializeField]
    public List<Rule> rules;

    [System.Serializable]
    public class Rule {
        public Rule(int size) {
            array = new bool[9];
        }

        public bool[] array;
    }

    public bool IsThisTile(MapTile tile, MapTile[,] mapTile) {
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

[CustomEditor(typeof(SO_RuleTile))]
public class LevelEditor:Editor {

    public Object source;

    public bool showMap = true;

    int firstDimensionSize;
    int previousSize;

    bool editMode;

    string confirmation;

    void SetUpArray(SO_RuleTile levels) {
        if(levels.rules != null && levels.rules.Count > 0) {
            for(int i = 0;i < levels.rules.Count;i++) {
                EditorGUILayout.LabelField("Rule number " + i);

                EditorGUILayout.BeginHorizontal();

                levels.rules[i].array[6] = EditorGUILayout.Toggle(levels.rules[i].array[6], GUILayout.Width(20));
                levels.rules[i].array[7] = EditorGUILayout.Toggle(levels.rules[i].array[7], GUILayout.Width(20));
                levels.rules[i].array[8] = EditorGUILayout.Toggle(levels.rules[i].array[8], GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                levels.rules[i].array[3] = EditorGUILayout.Toggle(levels.rules[i].array[3], GUILayout.Width(20));
                levels.rules[i].array[4] = EditorGUILayout.Toggle(levels.rules[i].array[4], GUILayout.Width(20));
                levels.rules[i].array[5] = EditorGUILayout.Toggle(levels.rules[i].array[5], GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                levels.rules[i].array[0] = EditorGUILayout.Toggle(levels.rules[i].array[0], GUILayout.Width(20));
                levels.rules[i].array[1] = EditorGUILayout.Toggle(levels.rules[i].array[1], GUILayout.Width(20));
                levels.rules[i].array[2] = EditorGUILayout.Toggle(levels.rules[i].array[2], GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    bool CanCreateNewArray() {
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Create New Array")) editMode = true;
        if(GUILayout.Button("Cancel")) editMode = false;
        EditorGUILayout.EndHorizontal();

        return editMode;
    }

    void CreateArray(SO_RuleTile levels) {
        if(levels.rules == null || levels.rules.Capacity == 0) {
            levels.rules = new List<SO_RuleTile.Rule>();
        }

        if(firstDimensionSize == previousSize) return;

        Debug.Log("First " + firstDimensionSize + ", previous " + previousSize);

        if(firstDimensionSize < previousSize) {
            for(int i = previousSize;i > firstDimensionSize;i--) {
                Debug.Log(i);
                levels.rules.RemoveAt(i - 1);
            }
        }

        if(firstDimensionSize > previousSize) {
            Debug.Log("Should add");
            Debug.Log(firstDimensionSize + "  " + levels.rules.Capacity);
            for(int i = 0;i < firstDimensionSize - previousSize;i++) {
                levels.rules.Add(new SO_RuleTile.Rule(9));
            }
        }

        previousSize = firstDimensionSize;

        //for(int i = 0;i < firstDimensionSize;i++) {
        //    levels.rules.Add(new SO_RuleTile.Rule(9));
        //}
    }

    void CreateNewArray(SO_RuleTile levels) {
        GetDimensions();
        if(ConfirmedCanCreate()) CreateArray(levels);
    }

    bool ConfirmedCanCreate() {
        EditorGUILayout.BeginHorizontal();
        bool canCreate = (GUILayout.Button("Create New Multidimensional Array"));
        EditorGUILayout.EndHorizontal();

        if(canCreate) {
            confirmation = "";
            editMode = false;
            return true;
        }
        return false;
    }

    void GetDimensions() {
        firstDimensionSize = EditorGUILayout.IntField("Number of rules", firstDimensionSize);
    }

    public override void OnInspectorGUI() {


        SO_RuleTile levels = (SO_RuleTile)target;

        if(CanCreateNewArray()) CreateNewArray(levels);

        levels.tile = EditorGUILayout.ObjectField("Tile associated : ", levels.tile, typeof(Object), false) as Tile;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Set of rules for center tiles");
        EditorGUILayout.LabelField("(Toggle on means it's a solid tile)");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Number of rules");

        SetUpArray(levels);

        serializedObject.Update();
        
    }
}
