using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [Header("Sprites and animation")]
    [SerializeField]
    public Sprite spriteRight;
    [SerializeField]
    public Sprite spriteLeft;

    [Header("Firing variables")]
    [SerializeField]
    public float delayBetweenShots;
    [SerializeField]
    public float efficiencyAgainstMonster;
    [SerializeField]
    public float efficiencyAgainstTiles;
    
    [SerializeField]
    public GameObject bulletPrefab;

    [Header("Bullet")]
    [SerializeField]
    public float speed;
    [SerializeField]
    public float damagePerBullet;

    public bool bounceBullet;

    public int nbBullet;

    public Color colorBullet;

    [System.Serializable]
    enum AttackType {
        RANGE,
        FIRING
    }
    [SerializeField]
    AttackType attackType = AttackType.FIRING;

    Vector3 position;

    [SerializeField]
    float range;

    public void Attack(Vector3 pos, Vector3 dir) {
        switch(attackType) {
            case AttackType.RANGE:
                position = pos + new Vector3(0, -0.1f, 0);

                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);

                foreach(Collider2D collider in colliders) {

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Tilemap")) {
                        MapTile t = FindObjectOfType<MapController>().GetTile(new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)));

                        if(t != null) {
                            FindObjectOfType<MapController>().AttackTile(t, damagePerBullet * efficiencyAgainstTiles);
                        }
                    }

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Monster")) {
                        if(collider.gameObject.GetComponent<CrawlerController>()) {
                            collider.gameObject.GetComponent<CrawlerController>().TakeDamage(damagePerBullet * efficiencyAgainstMonster);
                        }
                    }

                    if(collider.gameObject.layer == LayerMask.NameToLayer("Item")) {
                        if(collider.gameObject.GetComponent<LifeContainer>()) {
                            collider.gameObject.GetComponent<LifeContainer>().TakeDamage(damagePerBullet * efficiencyAgainstMonster);
                        }
                    }
                }

                break;

            case AttackType.FIRING:
                if(nbBullet == 1) {
                    GameObject instance = (GameObject)Instantiate(this.bulletPrefab, pos + new Vector3(0, -0.1f, 0), Quaternion.identity);
                    instance.GetComponent<Rigidbody2D>().velocity = dir.normalized * speed;
                    instance.GetComponent<SpriteRenderer>().color = colorBullet;
                    instance.GetComponent<Bullet>().bounce = bounceBullet;

                    instance.transform.position += new Vector3(dir.normalized.y, -dir.normalized.x) * Random.Range(-0.1f, 0.1f);
                }
                break;
        }
    }
}
