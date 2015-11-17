﻿using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {


	public bool equip;
	public WeaponManager.WeaponType weaponType;

	public int MaxAmmo;
	public int MaxClipAmmo = 30;
	public int currAmmo;
	public bool CanBurst;

	public GameObject HandPosition;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	GameObject bulletSpawnGO;
	ParticleSystem bulletPart;
	WeaponManager parentControl;

	bool fireBullet;
	AudioSource audioSource;
	Animator weaponAnim;

	[Header("Positions")]
	public bool hasOwner;
	public Vector3 EquipPosition;
	public Vector3 EquipRotation;
	public Vector3 UnEquipPosition;
	public Vector3 UnEquipRotation;
	//Debug Scale
	Vector3 scale;

	public RestPosition restPosition;
	public enum RestPosition{
		RightHip,
		Waist
	}

	// Use this for initialization
	void Start () {
		currAmmo = MaxClipAmmo;
		bulletSpawnGO = Instantiate (bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		bulletSpawnGO.AddComponent<ParticleDirection> ();
		bulletSpawnGO.GetComponent<ParticleDirection> ().weapon = bulletSpawn;
		bulletPart = bulletSpawnGO.GetComponent<ParticleSystem> ();

		audioSource = GetComponent<AudioSource> ();
		weaponAnim = GetComponent<Animator> ();
		scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = scale;
		if (equip) {
			transform.parent = transform.GetComponentInParent<WeaponManager> ().transform.GetComponent<Animator> ().GetBoneTransform (HumanBodyBones.RightHand);
			transform.localPosition = EquipPosition;
			transform.localRotation = Quaternion.Euler (EquipRotation);
			if (fireBullet) {
				if (currAmmo > 0) {

					currAmmo--;
					bulletPart.Emit (1);
					audioSource.Play ();
					if(weaponType == WeaponManager.WeaponType.Pistol)
						weaponAnim.SetTrigger("Fire");
					fireBullet = false;
				} else {
					if (MaxAmmo >= MaxClipAmmo) {
						currAmmo = MaxClipAmmo;
						MaxAmmo -= MaxClipAmmo;
					} else {
						currAmmo = MaxClipAmmo - (MaxClipAmmo - MaxAmmo);
					}

					fireBullet = false;
					Debug.Log ("Reload");
					
				}
			} 
		} else {
			if(hasOwner){
				switch(restPosition){
					case RestPosition.RightHip:
						transform.parent = transform.GetComponentInParent<WeaponManager> ().transform.GetComponent<Animator> ().GetBoneTransform (HumanBodyBones.RightUpperLeg);
						break;
					case RestPosition.Waist:
						transform.parent = transform.GetComponentInParent<WeaponManager> ().transform.GetComponent<Animator> ().GetBoneTransform (HumanBodyBones.Spine);
						break;
				}

				transform.localPosition = UnEquipPosition;
				transform.localRotation = Quaternion.Euler(UnEquipRotation);
			}
		}
	}

	public void Fire(){
		fireBullet = true;
	}
}
