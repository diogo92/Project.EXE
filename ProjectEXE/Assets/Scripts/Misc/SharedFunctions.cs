using UnityEngine;
using System.Collections;

public class SharedFunctions : MonoBehaviour {
	
	public static void PickupItem(GameObject owner, GameObject item){
		ItemID.ItemType id = item.GetComponent<ItemID> ().itemType;
		switch(id){
		case ItemID.ItemType.Health:
			break;
		case ItemID.ItemType.Weapon:
			WeaponManager wM = owner.GetComponent<WeaponManager>();
			if(wM.WeaponList.Count < wM.MaxCarryingWeapons){
				wM.WeaponList.Add(item);
			}
			else{
				wM.ActiveWeapon.equip = false;
				wM.ActiveWeapon.hasOwner = false;
				wM.ActiveWeapon.transform.parent = null;
				
				GameObject removeWeapon = null;
				
				foreach(GameObject go in wM.WeaponList){
					if(go == wM.ActiveWeapon.gameObject){
						removeWeapon = go;
					}
				}
				wM.WeaponList.Remove(removeWeapon);
				wM.WeaponList.Add(item);
			}
			
			item.transform.parent = owner.transform;
			item.GetComponent<WeaponControl>().hasOwner = true;
			break;
		case ItemID.ItemType.Wearable:
			break;
		case ItemID.ItemType.Power:
			if(owner.GetComponent<UserInput>().holdingPower != null){
				GameObject go = Instantiate((Object)owner.GetComponent<UserInput>().holdingPower,owner.transform.position,Quaternion.identity) as GameObject;
				go.SetActive(true);
			}
			owner.GetComponent<UserInput>().holdingPower = item;
			item.transform.parent=owner.transform;
			item.transform.position=owner.transform.position;
			item.SetActive(false);
			break;
		}
	}
	
	public static bool CheckAmmo(WeaponControl AW){
		if (AW.currAmmo > 0) {
			return true;
		} else
			return false;
	}
	
}
