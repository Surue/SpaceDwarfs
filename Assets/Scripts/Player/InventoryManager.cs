using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    public Image imageCurrentWeapon;
    public Text damageCurrentWeapon;
    public Text speedCurrentWeapon;

    // Use this for initialization
    //void Start () {
    //    if(PlayerInfo.Instance.currentWeapon == null) {
    //        PlayerInfo.Instance.currentWeapon = WeaponGenerator.Instance.GenerateWeapon();
    //    }
    //    imageCurrentWeapon.sprite = PlayerInfo.Instance.currentWeapon.spriteRight;

    //    damageCurrentWeapon.text = PlayerInfo.Instance.currentWeapon.damagePerBullet.ToString();
    //    speedCurrentWeapon.text = PlayerInfo.Instance.currentWeapon.speed.ToString();

   // }
	
	// Update is called once per frame
	void Update () {
		
	}
}
