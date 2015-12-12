using UnityEngine;
using System.Collections;

public class ItemID : MonoBehaviour {

	public ItemType itemType;

	public enum ItemType{
		Weapon,
		Health,
		Wearable,
		Power
	}

	/*
	 * POWERS - 
	 * 1 - Double Jump
	 * 
	 * */
	public int power;
}
