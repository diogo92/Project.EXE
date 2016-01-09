using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour
{
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
//Camera
//float cameraForward;

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
/*	if (anim.GetFloat ("Forward") > 0.5f) {
	if (Input.GetButtonDown ("Crouch")) {
		anim.SetTrigger ("Vault");
	}
}*/

		if (holdingPower != null && holdingPower.GetComponent<ItemID> ().power == 3) {
			if(Input.GetKeyDown(KeyCode.LeftControl) && freeze){
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
/*float sizeCurve = anim.GetFloat ("ColliderSize");
float newYcenter = 0.1f;

float lerpCenter = Mathf.Lerp (0.1f, newYcenter, sizeCurve);
col.center = new Vector3 (0, lerpCenter , 0);

col.height = Mathf.Lerp (startHeight, 0.5f, sizeCurve);*/
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
	//		aim = Input.GetMouseButton (1);


		weaponManager.aim = aim;
//if (aim) {
		if (holdingPower != null && Input.GetMouseButton(1) && holdingPower.GetComponent<ItemID> ().power == 2 ) {
			GetComponent<XrayPower> ().XRayPower ();
			GetComponent<CharacterSoundController>().PlayXray();
			crosshair.SetActive (true);
		} else {
			GetComponent<CharacterSoundController>().StopXray();
			crosshair.SetActive (false);
		}
		bool canFire = SharedFunctions.CheckAmmo (weaponManager.ActiveWeapon);
		//if(!weaponManager.ActiveWeapon.CanBurst){
		if (Input.GetMouseButtonDown (0) && attack/* && !anim.GetCurrentAnimatorStateInfo(2).IsTag("Reload") || debugShoot*/) {
			GetComponent<CharacterSoundController>().PlayPunch();
			StartCoroutine ("resetAttack");
			anim.SetTrigger ("Fire");
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
			/*if(canFire ){
			//	anim.SetTrigger("Fire");
				//ShootRay();
			//	cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon,true);
			//	weaponManager.FireActiveWeapon();
			//	weaponManager.ActiveWeapon.currAmmo--;
			}
			else{
			//	weaponManager.ReloadActiveWeapon();
			//	anim.SetTrigger("Reload");
			}*/

			//}
			/*//}else{
		if(Input.GetMouseButton(0) && !anim.GetCurrentAnimatorStateInfo(2).IsTag("Reload") || debugShoot){

			if(canFire ){
				anim.SetTrigger("Fire");
				ShootRay();
				cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon,true);
				//	weaponManager.FireActiveWeapon();
				weaponManager.ActiveWeapon.currAmmo--;
			}
			else{
				weaponManager.ReloadActiveWeapon();
				anim.SetTrigger("Reload");
			}
		}
	}
}
else{
	if(holdingPower != null && holdingPower.GetComponent<ItemID>().power == 2){
		GetComponent<XrayPower>().StopXRay();
	}
}*/

/*if(Input.GetAxis("Mouse ScrollWheel") !=0 ){
	if(Input.GetAxis("Mouse ScrollWheel") <= -0.1f){
		weaponManager.ChangeWeapon(false);
	}
	
	if(Input.GetAxis("Mouse ScrollWheel") > -0.1f){
		weaponManager.ChangeWeapon(true);
	}
}*/
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

	void ShootRay ()
	{
		float x = Screen.width / 2;
		float y = Screen.height / 2;

		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (x, y, 0));
		RaycastHit hit;

		GameObject go = Instantiate (bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		LineRenderer line = go.GetComponent<LineRenderer> ();
		Vector3 startPos = weaponManager.ActiveWeapon.bulletSpawn.TransformPoint (Vector3.zero);
		Vector3 endPos = Vector3.zero;

		int mask = ~(1 << 9);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask)) {
			float distance = Vector3.Distance (weaponManager.ActiveWeapon.bulletSpawn.transform.position, hit.point);
			RaycastHit[] hits = Physics.RaycastAll (startPos, hit.point - startPos, distance);

			foreach (RaycastHit hit2 in hits) {
				if (hit2.transform.GetComponent<Rigidbody> ()) {
					Vector3 direction = hit2.transform.position - transform.position;
					direction = direction.normalized;
					if (hit2.transform.GetComponent<CharacterStats> () && hit2.collider.name != "Sight")
						hit2.transform.GetComponent<CharacterStats> ().Health--;
					hit2.transform.GetComponent<Rigidbody> ().AddForce (direction * 200);
				} else if (hit2.transform.GetComponent<Destructible> ()) {
					hit2.transform.GetComponent<Destructible> ().destruct = true;
				}
			}

			endPos = hit.point;
		} else {
			endPos = ray.GetPoint (100);
		}
		line.SetPosition (0, startPos);
		line.SetPosition (1, endPos); 
	}

	void LateUpdate ()
	{


		aimingWeight = Mathf.MoveTowards (aimingWeight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);

		Vector3 normalState = new Vector3 (0, 0, -2f);
		Vector3 aimingState = new Vector3 (0, 0, -2f);

		Vector3 pos = Vector3.Lerp (normalState, aimingState, aimingWeight);

		cam.transform.localPosition = pos;

/*	if (aim) {
	Vector3 eulerAngleOffset = Vector3.zero;
	eulerAngleOffset = new Vector3(ik.aimingX,ik.aimingY,ik.aimingZ);

	Ray ray = new Ray(cam.position, cam.forward);

	Vector3 lookPosition = ray.GetPoint(ik.point);

	ik.spine.LookAt(lookPosition);
	ik.spine.Rotate(eulerAngleOffset,Space.Self);
}*/
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

//if (!aim) {
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
/*	} else {

	move = Vector3.zero;

	Vector3 dir = lookPos - transform.position;

	dir.y = 0;

	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

	anim.SetFloat("Forward",vertical);
	anim.SetFloat("Turn", horizontal);

}*/
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