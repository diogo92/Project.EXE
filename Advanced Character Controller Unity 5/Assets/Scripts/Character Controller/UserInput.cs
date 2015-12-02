using UnityEngine;
using System.Collections;


public class UserInput : MonoBehaviour
{

    public bool walkByDefault = false;

    private CharMove character;
    private Transform cam;
    private Vector3 camForward;
    private Vector3 move;

	//Camera
	//float cameraForward;
	public bool aim;
	public float aimingWeight;

	public bool lookInCameraDirection;
	Vector3 lookPos;

	WeaponManager weaponManager;

	public bool debugShoot;

	WeaponManager.WeaponType weaponType;

	CapsuleCollider col;
	float startHeight;
	Animator anim;
	//Ik stuff

	[SerializeField] public IK ik;
	[System.Serializable] public class IK{
		public Transform spine;
		public float aimingZ = 213.46f;
		public float aimingX = -65.97f;
		public float aimingY = 20;
		public float point = 30;
		public bool DebugAim;
	}

	FreeCameraLook cameraFunctions;

    void Start()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }

        character = GetComponent<CharMove>();

		anim = GetComponent<Animator> ();

		weaponManager = GetComponent<WeaponManager> ();

		col = GetComponent<CapsuleCollider>();
		startHeight = col.height;

		cameraFunctions = Camera.main.GetComponentInParent<FreeCameraLook> ();

		offsetCross = cameraFunctions.crosshairOffsetWiggle;
    }

	void CorrectIK(){
		weaponType = weaponManager.weaponType;

		if (!ik.DebugAim) {
			switch(weaponType){
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


	void AdditionalInput(){

		if (anim.GetFloat ("Forward") > 0.5f) {
			if (Input.GetButtonDown ("Crouch")) {
				anim.SetTrigger ("Vault");
			}
		}
	}

	void HandleCurves(){
		float sizeCurve = anim.GetFloat ("ColliderSize");
		float newYcenter = 0.3f;

		float lerpCenter = Mathf.Lerp (1f, newYcenter, sizeCurve);
		col.center = new Vector3 (0, lerpCenter , 0);

		col.height = Mathf.Lerp (startHeight, 0.5f, sizeCurve);
	}

	void Update(){



		CorrectIK ();
		if(!ik.DebugAim)
		aim = Input.GetMouseButton (1);

		if(Input.GetKey(KeyCode.Space)){
			anim.SetTrigger("Macarena");
		}
		weaponManager.aim = aim;
		if (aim) {
			if(!weaponManager.ActiveWeapon.CanBurst){
				if(Input.GetMouseButtonDown(0) || debugShoot){
					anim.SetTrigger("Fire");
					ShootRay();
					cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon,true);
				//	weaponManager.FireActiveWeapon();

				}
			}else{
				if(Input.GetMouseButton(0) || debugShoot){
					anim.SetTrigger("Fire");
					ShootRay();
					cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon,true);

				//	weaponManager.FireActiveWeapon();
				}
			}
		}
		if(Input.GetAxis("Mouse ScrollWheel") !=0 ){
			if(Input.GetAxis("Mouse ScrollWheel") <= -0.1f){
				weaponManager.ChangeWeapon(false);
			}
			
			if(Input.GetAxis("Mouse ScrollWheel") > -0.1f){
				weaponManager.ChangeWeapon(true);
			}
		}
		
		AdditionalInput ();
		HandleCurves ();
	}

	public GameObject bulletPrefab;

	void ShootRay(){
		float x = Screen.width / 2;
		float y = Screen.height / 2;

		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (x, y, 0));
		RaycastHit hit;

		GameObject go = Instantiate (bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		LineRenderer line = go.GetComponent<LineRenderer> ();
		Vector3 startPos = weaponManager.ActiveWeapon.bulletSpawn.TransformPoint (Vector3.zero);
		Vector3 endPos = Vector3.zero;

		int mask = ~(1 << 8);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask)) {
			float distance = Vector3.Distance (weaponManager.ActiveWeapon.bulletSpawn.transform.position, hit.point);
			RaycastHit[] hits = Physics.RaycastAll (startPos, hit.point - startPos, distance);

			foreach (RaycastHit hit2 in hits) {
				if (hit2.transform.GetComponent<Rigidbody> ()) {
					Vector3 direction = hit2.transform.position - transform.position;
					direction = direction.normalized;
					hit2.transform.GetComponent<Rigidbody> ().AddForce (direction * 200);
				}
				else if (hit2.transform.GetComponent<Destructible>()){
					hit2.transform.GetComponent<Destructible>().destruct = true;
				}
			}

			endPos = hit.point;
		} else {
			endPos = ray.GetPoint(100);
		}
		line.SetPosition (0, startPos);
		line.SetPosition (1, endPos); 
	}

	void LateUpdate(){


		aimingWeight = Mathf.MoveTowards (aimingWeight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);

		Vector3 normalState = new Vector3 (0, 0, -2f);
		Vector3 aimingState = new Vector3 (0, 0, -0.5f);

		Vector3 pos = Vector3.Lerp (normalState, aimingState, aimingWeight);

		cam.transform.localPosition = pos;

		if (aim) {
			Vector3 eulerAngleOffset = Vector3.zero;
			eulerAngleOffset = new Vector3(ik.aimingX,ik.aimingY,ik.aimingZ);

			Ray ray = new Ray(cam.position, cam.forward);

			Vector3 lookPosition = ray.GetPoint(ik.point);

			ik.spine.LookAt(lookPosition);
			ik.spine.Rotate(eulerAngleOffset,Space.Self);
		}
	}


	float horizontal;
	float vertical;
	float offsetCross;
    void FixedUpdate()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


		if (horizontal < -offsetCross || horizontal > offsetCross || vertical < -offsetCross || vertical > offsetCross) {
			cameraFunctions.WiggleCrosshairAndCamera(weaponManager.ActiveWeapon,false);
		}

		if (!aim) {
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
		} else {

			move = Vector3.zero;

			Vector3 dir = lookPos - transform.position;

			dir.y = 0;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

			anim.SetFloat("Forward",vertical);
			anim.SetFloat("Turn", horizontal);

		}
        if (move.magnitude > 1) // make sure the movement is normalizd
            move.Normalize();

        bool walkToggle = Input.GetKey(KeyCode.LeftShift) || aim;

        float walkMultiplier = 1;

        if (walkByDefault)
        {
            if (walkToggle)
            {
                walkMultiplier = 1;
            }
            else
            {
                walkMultiplier = 0.5f;
            }
        }
        else
        {
            if (walkToggle)
            {
                walkMultiplier = 0.5f;
            }
            else
            {
                walkMultiplier = 1;
            }
        }

		lookPos = lookInCameraDirection && cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;

        move *= walkMultiplier;
        character.Move(move,aim,lookPos);
    }

}