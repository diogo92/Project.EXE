﻿using UnityEngine;
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
	 * 2 - Xray
	 * 3 - Time freeze
	 * */
	public int power;
}
