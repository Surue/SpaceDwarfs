using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "item_name", menuName = "Gun / Gun")]
[System.Serializable]
public class SO_Gun : ScriptableObject {
    [Header("Sprites and animation")]
    [SerializeField]
    public Sprite spriteRight;
    [SerializeField]
    public Sprite spriteLeft;
    [SerializeField]
    public RuntimeAnimatorController animator;

    [Header("Firing variables")]
    [SerializeField]
    public float delayBetweenShots;
    [Range(0, 1)]
    [SerializeField]
    float efficiencyAgainstMonster;
    [Range(0, 1)]
    [SerializeField]
    float efficiencyAgainstTiles;

    [Header("Attack type")]
    [SerializeField]
    AttackType attackType;
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    float range;

    [Header("Range damage")]
    [SerializeField]
    float damage;

    [Header("Bullet")]
    [SerializeField]
    float speed;

    [System.Serializable]
    enum AttackType {
        RANGE,
        FIRING
    }

    Vector3 position;

    public void Attack(Vector3 pos, Vector3 dir) {
        switch(attackType) {
            case AttackType.RANGE:
                position = pos + new Vector3(0, -0.1f, 0);

                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);

                foreach(Collider2D collider in colliders) {

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Tilemap")) {
                        MapTile t = FindObjectOfType<MapController>().GetTile(new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)));

                        if(t != null) {
                            FindObjectOfType<MapController>().AttackTile(t, damage * efficiencyAgainstTiles);
                        }
                    } 

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Monster")) {
                        if(collider.gameObject.GetComponent<CrawlerController>()) {
                            collider.gameObject.GetComponent<CrawlerController>().TakeDamage(damage * efficiencyAgainstMonster);
                        }
                    }

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Item")) {
                        if(collider.gameObject.GetComponent<LifeContainer>()) {
                            collider.gameObject.GetComponent<LifeContainer>().TakeDamage(damage * efficiencyAgainstMonster);
                        }
                    }
                }

                break;

            case AttackType.FIRING:
                GameObject instance = (GameObject)Instantiate(this.bullet, pos + new Vector3(0, -0.1f, 0), Quaternion.identity);
                instance.GetComponent<Rigidbody2D>().velocity = dir.normalized * speed;
                break;
        }
    }
}
