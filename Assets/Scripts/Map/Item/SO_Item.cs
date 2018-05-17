using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item_name", menuName = "Tilemap / Item")]
[System.Serializable]
public class SO_Item : ScriptableObject {

    public GameObject prefab;
    [Range(1, 100)]
    public float apparitionRatePerRegion;

    [Header("Spawn conditions")]
    public bool closedRegion;
    public bool nestRegion;
    public bool normalRegion;
    public bool spawnPlayerRegion;

    public bool CanBeInThisRegion(MapRegion region) {
        switch(region.type) {
            case MapRegion.TypeRegion.CLOSED:
                if(closedRegion) return true;
                break;

            case MapRegion.TypeRegion.NEST:
                if(nestRegion) return true;
                break;

            case MapRegion.TypeRegion.NORMAL:
                if(normalRegion) return true;
                break;

            case MapRegion.TypeRegion.PLAYER_SPAWN:
                if(spawnPlayerRegion) return true;
                break;
        }

        return false;
    }
}
