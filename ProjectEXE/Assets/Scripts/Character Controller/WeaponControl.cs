using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ItemID))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponControl : MonoBehaviour {


	public bool equip;
	public WeaponManager.WeaponType weaponType;

	public int curCarryingAmmo;
	public int maxCarryingAmmo = 50;

	public int MaxAmmo;
	public int MaxClipAmmo = 30;
	public int currAmmo;
	public bool CanBurst;
	public float Kickback = 0.1f;

	public GameObject HandPosition;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	GameObject bulletSpawnGO;
	ParticleSystem bulletPart;
	WeaponManager parentControl;

	bool fireBullet;
	AudioSource audioSource;
	Animator weaponAnim;
	Rigidbody rigidBody;
	BoxCollider boxCol;
	PickableItem pickableItem;

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

		curCarryingAmmo = maxCarryingAmmo;
		currAmmo = MaxClipAmmo;

		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.isKinematic = true;
		boxCol = GetComponent<BoxCollider> ();
		pickableItem = GetComponentInChildren<PickableItem> ();

		audioSource = GetComponent<AudioSource> ();
		weaponAnim = GetComponent<Animator> ();
		scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = scale;
		if (equip) {
			if(pickableItem.gameObject.activeInHierarchy){
				pickableItem.gameObject.SetActive(false);
			}
			transform.parent = transform.GetComponentInParent<WeaponManager> ().transform.GetComponent<Animator> ().GetBoneTransform (HumanBodyBones.RightHand);
			transform.localPosition = EquipPosition;
			transform.localRotation = Quaternion.Euler (EquipRotation);
		} else {
			if(hasOwner){

				if(pickableItem.gameObject.activeInHierarchy){
					pickableItem.gameObject.SetActive(false);
				}

				boxCol.isTrigger = true;
				rigidBody.isKinematic = true;

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
			else{
				if(!pickableItem.gameObject.activeInHierarchy){
					pickableItem.gameObject.SetActive(true);
				}

				boxCol.isTrigger = false;
				rigidBody.isKinematic = true;

				if(pickableItem.CharacterInTrigger){
					if(pickableItem.Owner.GetComponent<UserInput>()){
						pickableItem.Owner.GetComponent<UserInput>().CanPickUp = true;
						pickableItem.Owner.GetComponent<UserInput>().Item = this.gameObject;
					}
				}
				else{
					if(pickableItem.Owner){
						if(pickableItem.Owner.GetComponent<UserInput>()){
							pickableItem.Owner.GetComponent<UserInput>().CanPickUp = false;
							pickableItem.Owner.GetComponent<UserInput>().Item = null;
							pickableItem.Owner = null;
						}
					}
				}
			}
		}
	}

	public void Fire(){
		fireBullet = true;
	}
}
