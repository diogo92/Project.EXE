using UnityEngine;
using System.Collections;

public class CharMove : MonoBehaviour
{

    public float moveSpeedMultiplier = 1;
    float stationaryTurnSpeed = 180; 
    float movingTurnSpeed = 360;

    public bool onGround; 

    Animator animator;

    Vector3 moveInput; 
    float turnAmount; 
    float forwardAmount; 
    Vector3 velocity;
    float jumpPower = 10;
   
    IComparer rayHitComparer;
    Rigidbody rigidBody;

	float lastAirTime;
	Collider col;
	public PhysicMaterial highFriction;
	public PhysicMaterial lowFriction;

	float autoTurnThreshold = 10;
	float autoTurnSpeed = 20;
	bool aim;
	Vector3 currentLookPos;
   
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        SetUpAnimator();

        rigidBody = GetComponent<Rigidbody>();  

		col = GetComponent<Collider> ();
    }

    void SetUpAnimator()
    {
        animator = GetComponent<Animator>();

        foreach (var childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != animator)
            {
                animator.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break; 
            }
        }

    }
 
    void OnAnimatorMove()
    {
        if (onGround && Time.deltaTime > 0)
        {
            Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime; 
			//Delta position (position difference) - The difference in position between last frame and current one
			//Speed = position difference of the animator * speed multiplier / time
            v.y = rigidBody.velocity.y; //store the characters vertical speed, so it does not affect jump speed
            rigidBody.velocity = v; // update character's speed
        }
    }

    public void Move(Vector3 move, bool aim, Vector3 lookPos)
    {    
		//Vector3 move is the input in the world space
        if (move.magnitude > 1) //Make sure that the movement is normalized
            move.Normalize();
       
        this.moveInput = move; //Store the move
		this.aim = aim;
		this.currentLookPos = lookPos;

        velocity = rigidBody.velocity; //Store current velocity

        ConvertMoveInput();

		if (!aim) {
			ApplyExtraTurnRotation ();
			TurnTowardsCameraForward ();
		}


        ApplyExtraTurnRotation();
        GroundCheck ();
		SetFriction ();

		if (onGround) {
			HandleGroundVelocities ();
		} else {

			HandleAirborneVelocities();
		}
        UpdateAnimator();

    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        turnAmount = Mathf.Atan2(localMove.x, localMove.z);

        forwardAmount = localMove.z;
    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void UpdateAnimator()
    { 
		animator.applyRootMotion = true;

		if (!aim) {
			animator.SetFloat ("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat ("Turn", turnAmount, 0.1f, Time.deltaTime);
		}
		animator.SetBool("Aim",aim);
		animator.SetBool ("OnGround", onGround);
		
    }
   
    void GroundCheck()
    {
        Ray ray = new Ray(transform.position + Vector3.up * .1f, -Vector3.up);

        RaycastHit[] hits = Physics.RaycastAll(ray, .1f);
        rayHitComparer = new RayHitComparer();
        System.Array.Sort(hits, rayHitComparer);

        if (velocity.y < jumpPower * .5f)
        { 	
            onGround = false;
            rigidBody.useGravity = true;

            foreach (var hit in hits)
            { 
                if (!hit.collider.isTrigger)
                {
                    if (velocity.y <= 0)
                    {
                          rigidBody.position = Vector3.MoveTowards(rigidBody.position, hit.point, Time.deltaTime * 100);
                    }

                    onGround = true; 
                    rigidBody.useGravity = false; 
                }
            }
        }

		if (!onGround) {
			lastAirTime = Time.time;
		}
    }

	void TurnTowardsCameraForward(){
		if (Mathf.Abs (forwardAmount) < .01f) {
			Vector3 lookDelta = transform.InverseTransformDirection(currentLookPos - transform.position);

			float lookAngle = Mathf.Atan2(lookDelta.x,lookDelta.z) * Mathf.Rad2Deg;

			if(Mathf.Abs(lookAngle) > autoTurnThreshold){
				turnAmount += lookAngle * autoTurnSpeed * .001f;
			}
		}
	}


	void SetFriction(){
		if (onGround) {
			if (moveInput.magnitude == 0) {
				col.material = highFriction;
			} else {
				col.material = lowFriction;
			}
		} else {
			col.material = lowFriction;
		}
	}

	void HandleGroundVelocities(){
		velocity.y = 0;

		if (moveInput.magnitude == 0) {
			velocity.x = 0;
			velocity.z = 0;
		}
	}

	void HandleAirborneVelocities(){
		Vector3 airMove = new Vector3 (moveInput.x * 6, velocity.y, moveInput.z * 6);
		velocity = Vector3.Lerp (velocity, airMove, Time.deltaTime * 2);

		rigidBody.useGravity = true;

		Vector3 extraGravityForce = (Physics.gravity * 2);

		rigidBody.AddForce (extraGravityForce);
	}

    class RayHitComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
        }
    }

}