using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour
{
	//User input handler

	//Power stuff
	bool jump = false;
	public GameObject holdingPower;
	bool freeze = true;

	//Pickup stuff
	public bool CanPickUp;
	public GameObject Item;

	//Fight stuff
	public GameObject fightObject;
	bool attack = true;


	//Character controller and Camerastuff
	public bool walkByDefault = false;
	private CharMove character;
	private Transform cam;
	private Vector3 camForward;
	private Vector3 move;
	public bool lookInCameraDirection;
	Vector3 lookPos;

	//Shooting and Weapon stuff -- NOT USED
	public bool aim;
	public float aimingWeight;
	WeaponManager weaponManager;
	public bool debugShoot;
	WeaponManager.WeaponType weaponType;


	//Animator stuff
	CapsuleCollider col;
	float startHeight;
	Animator anim;

	//Ik stuff
	[SerializeField]
	public IK ik;
	[System.Serializable]
	public class IK
	{
		public Transform spine;
		public float aimingZ = 213.46f;
		public float aimingX = -65.97f;
		public float aimingY = 20;
		public float point = 30;
		public bool DebugAim;
	}

	FreeCameraLook cameraFunctions;


	//UI
	public GameObject pickText;
	public UnityEngine.UI.Text hp;
	public UnityEngine.UI.Text currPower;
	public GameObject crosshair;



	void Start ()
	{
		if (Camera.main != null) {
			cam = Camera.main.transform;
		}

		character = GetComponent<CharMove> ();

		anim = GetComponent<Animator> ();



		weaponManager = GetComponent<WeaponManager> ();

		col = GetComponent<CapsuleCollider> ();
		startHeight = col.height;

		cameraFunctions = Camera.main.GetComponentInParent<FreeCameraLook> ();

		offsetCross = cameraFunctions.crosshairOffsetWiggle;
	}

	void CorrectIK ()
	{
		weaponType = weaponManager.weaponType;

		if (!ik.DebugAim) {
			switch (weaponType) {
			case WeaponManager.WeaponType.Pistol:
				ik.aimingZ = 212.19f;
				ik.aimingX = -63.8f;
				ik.aimingY = 16.65f;
				break;
			case WeaponManager.WeaponType.Rifle:
				ik.aimingZ = 212.19f;
				ik.aimingX = -63.1f;
				ik.aimingY = 14.1f;
				break;

			}
		}
	}


	IEnumerator Freeze ()
	{
		//freeze
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Animator>().enabled = false;
			enemy.GetComponent<CharMove>().enabled = false;
			enemy.GetComponent<EnemyAI>().enabled = false;
			enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			
		}
		print ("Frozen");
		yield return new WaitForSeconds (5f);
		//unfreeze
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Animator>().enabled = true;
			enemy.GetComponent<CharMove>().enabled = true;
			enemy.GetComponent<EnemyAI>().enabled = true;
			enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}
		print ("Unfrozen");
		yield return new WaitForSeconds (10f);
		print ("Freeze ready");
		freeze = true;
	}

	void AdditionalInput ()
	{

		if (holdingPower != null && holdingPower.GetComponent<ItemID> ().power == 3) {
			if(Input.GetMouseButtonDown(1) && freeze){
				freeze = false;
				StartCoroutine("Freeze");
			}
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			jump = true;
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			transform.position = new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z);
		}
	}

	void HandleCurves ()
	{
	}



	IEnumerator resetAttack ()
	{
		attack = false;
		yield return new WaitForSeconds (0.7f);
		attack = true;
	}



	void Update ()
	{

		CorrectIK ();
		if (!ik.DebugAim)


		weaponManager.aim = aim;
		if (holdingPower != null && Input.GetMouseButton(1) && holdingPower.GetComponent<ItemID> ().power == 2 ) {
			GetComponent<XrayPower> ().XRayPower ();
			GetComponent<CharacterSoundController>().PlayXray();
			crosshair.SetActive (true);
		} else {
			GetComponent<CharacterSoundController>().StopXray();
			crosshair.SetActive (false);
		}
		bool canFire = SharedFunctions.CheckAmmo (weaponManager.ActiveWeapon);
		if (Input.GetMouseButtonDown (0) && attack) {
			GetComponent<CharacterSoundController>().PlayPunch();
			StartCoroutine ("resetAttack");
			int r = Random.Range(0,3);
			switch (r){
			case 0:
				anim.SetTrigger ("Fire");
				break;
			case 1:
				anim.SetTrigger ("Fire1");
				break;
			case 2:
				anim.SetTrigger ("Fire2");
				break;
			}
			if (fightObject.GetComponent<FightObjectScript> ().enemies.Count > 0) {
				for(int i = fightObject.GetComponent<FightObjectScript> ().enemies.Count - 1;i >= 0; i--){
					if(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i] != null) && (!((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsTag("Dying") && !((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsTag("Dead"))){
					((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.GetComponent<Animator> ().SetTrigger("Take_Punch");
					((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.GetComponent<CharacterStats> ().Health--;
					if(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject == null)
						fightObject.GetComponent<FightObjectScript> ().enemies.RemoveAt (i);
					}
				}

			}

		}
		AdditionalInput ();
		HandleCurves ();
		PickupItem ();
		UpdateUI ();
	}

	void UpdateUI ()
	{
		hp.text = GetComponent<CharacterStats>().Health.ToString ();
		if (holdingPower == null)
			return;
	    switch (holdingPower.GetComponent<ItemID> ().power) {
		case 1:
			currPower.text = "Double Jump";
			break;
		case 2:
			currPower.text = "X-Ray";
			break;
		case 3:
			currPower.text = "Time Freeze";
			break;
		default:
			character.hasDoubleJumpAbility = false;
			break;
		}
	}


	void PickupItem ()
	{
		if (CanPickUp) {
			if (!pickText.activeInHierarchy) {
				pickText.SetActive (true);
			}
			if (Input.GetKeyUp (KeyCode.F)) {
				SharedFunctions.PickupItem (this.gameObject, Item);
				CanPickUp = false;
				GetComponent<CharacterSoundController>().PlayPickUp();
				if (holdingPower != null) {

					switch (holdingPower.GetComponent<ItemID> ().power) {
					case 1:
						character.hasDoubleJumpAbility = true;
						break;
					default:
						character.hasDoubleJumpAbility = false;
						break;
					}
				}

			}
		} else {
			if (pickText.activeInHierarchy) {
				pickText.SetActive (false);
			}
		}
	}
	public GameObject bulletPrefab;



	void LateUpdate ()
	{


		aimingWeight = Mathf.MoveTowards (aimingWeight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);

		Vector3 normalState = new Vector3 (0, 0, -2f);
		Vector3 aimingState = new Vector3 (0, 0, -2f);

		Vector3 pos = Vector3.Lerp (normalState, aimingState, aimingWeight);

		cam.transform.localPosition = pos;

	}

	void Die ()
	{
		anim.SetTrigger("Die");
		GetComponent<CharacterSoundController> ().PlayDie ();
		if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Dead")){
			this.gameObject.SetActive(false);
			Destroy(this.gameObject);
			Application.LoadLevel(Application.loadedLevel);
		}
	}


	float horizontal;
	float vertical;
	float offsetCross;
	void FixedUpdate ()
	{
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");


		if (horizontal < -offsetCross || horizontal > offsetCross || vertical < -offsetCross || vertical > offsetCross) {
			cameraFunctions.WiggleCrosshairAndCamera (weaponManager.ActiveWeapon, false);
		}

		if (cam != null) {
			//Take the forward vector of the camera (from its transform) and
			//eliminate the y component
			//scale the camera forward with the mask (1,0,1) to eliminate y and normalize it
			camForward = Vector3.Scale (cam.forward, new Vector3 (1, 0, 1)).normalized;
        
			//move input front/backward = forward direction of the camera * user input amount (vertical)
			//move input left/right = right direction of the camera * user input amount (horizontal)
			move = vertical * camForward + horizontal * cam.right;
		} else {

			//if there is not a camera, use the global forward (+z) and right ( +x)
			move = vertical * Vector3.forward + horizontal * Vector3.right;
		}

		if (move.magnitude > 1) // make sure the movement is normalizd
			move.Normalize ();

		bool walkToggle = Input.GetKey (KeyCode.LeftShift) || aim;
		float walkMultiplier = 1;

		if (walkByDefault) {
			if (walkToggle) {
				walkMultiplier = 1;
			} else {
				walkMultiplier = 0.5f;
			}
		} else {
			if (walkToggle) {
				walkMultiplier = 0.5f;
			} else {
				walkMultiplier = 1;
			}
		}	
		if ((horizontal != 0 || vertical !=0)  && anim.GetCurrentAnimatorStateInfo(0).IsTag("Grounded")){
			GetComponent<CharacterSoundController>().PlayWalk(walkToggle);
		}
		lookPos = lookInCameraDirection && cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;
		
		move *= walkMultiplier;
		character.Move (move, aim, lookPos, jump);
		jump = false;
		if (transform.position.y <= 7900) {
			Die ();
		}
		if (character.die) {
			Die ();
		}
	}

}