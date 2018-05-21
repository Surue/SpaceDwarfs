using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour {

    [System.Serializable]
    struct WeaponSprite {
        [SerializeField]
        public Sprite spriteRight;
        [SerializeField]
        public Sprite spriteLeft;
    }

    [Header("Sprites")]
    [SerializeField]
    List<WeaponSprite> weaponSprite;

    [Header("Bullet")]
    [SerializeField]
    List<GameObject> bulletPrefab;

    static WeaponGenerator instance;
    public static WeaponGenerator Instance {
        get {
            return instance;
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }

    public Weapon GenerateWeapon() {
        int difficulty = PlayerInfo.Instance.levelFinished;

        Weapon weapon = new Weapon();
        //Sprite
        int indexSprite = Random.Range(0, weaponSprite.Count);

        weapon.spriteLeft = weaponSprite[indexSprite].spriteLeft;
        weapon.spriteRight = weaponSprite[indexSprite].spriteRight;

        //Delay between shot
        weapon.delayBetweenShots = Random.Range(0.1f, 1f);
        weapon.efficiencyAgainstMonster = Random.Range(0.001f, 1);
        weapon.efficiencyAgainstTiles = Random.Range(0.001f, 1);

        //BUllet
        weapon.bulletPrefab = bulletPrefab[Random.Range(0, bulletPrefab.Count)];
        weapon.colorBullet = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        while(weapon.damagePerBullet <= 0) {
            weapon.damagePerBullet = difficulty + Random.Range(-1f, 2f);
        }
        
        weapon.speed = Random.Range(3f, 7f) + difficulty % 5;

        return weapon;
    }
}
