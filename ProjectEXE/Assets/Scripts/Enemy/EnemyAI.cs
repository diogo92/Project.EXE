using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour {

	NavMeshAgent agent;
	CharMove charMove;
	public GameObject fightObject;
	public GameObject waypointHolder;
	public List<Transform> waypoints = new List<Transform>();
	float targetTolerance = 1;	
	int waypointIndex;
	Vector3 targetPos;

	Animator anim;
	WeaponManager weaponManager;


	float patrolTimer;
	public float waitingTime = 4;
	public float attackRate = 1f;
	float attackTimer;

	public List<GameObject> Enemies = new List<GameObject>();
	public GameObject EnemyToAttack;


	public enum AIstate{
		Patrol,Attack
	}

	public AIstate aiState;
	// Use this for initialization
	void Start () {
		agent = GetComponentInChildren<NavMeshAgent> ();
		charMove = GetComponent<CharMove> ();
		anim = GetComponent<Animator> ();
		weaponManager = GetComponent<WeaponManager> ();

		if (waypointHolder) {
			Transform[] wayp = waypointHolder.GetComponentsInChildren<Transform> ();

			foreach (Transform tr in wayp) {
				if (tr != waypointHolder.transform) {
					waypoints.Add (tr);
				}
			}
		} 
	}
	
	// Update is called once per frame
	void Update () {

		if (GetComponent<CharacterStats> ().Health <= 0 ) {
			anim.SetBool("Dead",true);
			anim.SetTrigger("Die");
			if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Dead")){
				if(GetComponent<CharacterStats> ().dropable != null){

					GameObject go = Instantiate((Object)GetComponent<CharacterStats> ().dropable,new Vector3(transform.position.x,transform.position.y + 1, transform.position.z),Quaternion.identity) as GameObject;
					go.transform.parent=null;

				}
				this.gameObject.SetActive(false);
				Destroy(this.gameObject);
			}
			return;
		}
		DecideState ();

		switch (aiState) {
		case AIstate.Attack:
			Attack ();
			break;
		case AIstate.Patrol:
			Patrolling();
			break;
		}
	}

	void DecideState(){
		if (Enemies.Count > 0) {
			if(!EnemyToAttack){
				foreach(GameObject enGo in Enemies){
					if(enGo.tag=="Player")
						EnemyToAttack=enGo;
				}
			}
			else{
				aiState = AIstate.Attack;
			}
		}
	}

	void Attack(){
		agent.speed = 1.5f;
			agent.transform.position = transform.position;
			agent.destination = EnemyToAttack.transform.position;
			Vector3 velocity = agent.desiredVelocity * 0.5f;
			charMove.Move (velocity, false, targetPos, false);
		transform.LookAt(EnemyToAttack.transform.position);
		attackTimer += Time.deltaTime;
		
		if (attackTimer > attackRate) {

			anim.SetTrigger ("Fire");
			if (fightObject.GetComponent<FightObjectScript> ().enemies.Count > 0) {
				for(int i = fightObject.GetComponent<FightObjectScript> ().enemies.Count - 1;i >= 0; i--){
					if(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i] != null) && (!((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsTag("Dying"))){
						if(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject.transform.parent.tag != "Enemy")
							AttackEnemy(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject);
						
						if(((Collider)fightObject.GetComponent<FightObjectScript> ().enemies[i]).gameObject == null)
							fightObject.GetComponent<FightObjectScript> ().enemies.RemoveAt (i);
					}
				}
				
			}
			attackRate = (float)Random.Range(1,4);
			attackTimer = 0;
		}
		
	}

	void AttackEnemy(GameObject enemy){
		enemy.transform.parent.GetComponent<Animator> ().SetTrigger("Take_Punch");
		enemy.transform.parent.GetComponent<CharacterStats> ().Health-=10;
	}


	void OnAnimatorIK(){
		if (EnemyToAttack) {
			anim.SetLookAtWeight (1, 0.8f, 1, 1, 1);
			anim.SetLookAtPosition (EnemyToAttack.transform.position);
		} else {
			anim.SetLookAtWeight(0);
		}
	}

	void ShootRay(){
		RaycastHit hit;
		GameObject go = Instantiate (weaponManager.bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		LineRenderer line = go.GetComponent<LineRenderer> ();

		Vector3 startPos = weaponManager.ActiveWeapon.bulletSpawn.TransformPoint (Vector3.zero);
		Vector3 endPos = Vector3.zero;

		int mask = ~(1 << 9);

		Vector3 directionToAttack = EnemyToAttack.transform.position - transform.position;

		if (Physics.Raycast (startPos,directionToAttack,out hit, Mathf.Infinity, mask)) {
			float distance = Vector3.Distance(weaponManager.ActiveWeapon.bulletSpawn.transform.position,hit.point);
			RaycastHit[] hits = Physics.RaycastAll(startPos,hit.point - startPos, distance);

			foreach(RaycastHit hit2 in hits){
				if(hit2.transform.GetComponent<Rigidbody>()){
					Vector3 direction = hit2.transform.position - transform.position;
					direction = direction.normalized;
					hit2.transform.GetComponent<Rigidbody>().AddForce(direction * 200);
				}
				else if(hit2.transform.GetComponent<Destructible>()){
					hit2.transform.GetComponent<Destructible>().destruct = true;
				}
			}

			endPos = hit.point;
		}

		line.SetPosition (0, startPos);
		line.SetPosition (1, endPos);

	}


	void Patrolling(){
		agent.speed = 1;


		if(waypoints.Count > 0){
			if(agent.remainingDistance < agent.stoppingDistance){
				patrolTimer+= Time.deltaTime;

				if(patrolTimer >= waitingTime){
					if(waypointIndex == waypoints.Count - 1){
						waypointIndex = 0;
					}
					else{
						waypointIndex++;
					}
					patrolTimer = 0;
				}
			}
			else{
				patrolTimer = 0;
			}
			agent.transform.position = transform.position;

			agent.destination = waypoints[waypointIndex].position;

			Vector3 velocity = agent.desiredVelocity * 0.5f;

			charMove.Move(velocity,false,targetPos,false);
		}
		else{
			agent.transform.position = transform.position;

			Vector3 lookPos = (waypoints.Count > 0)? waypoints[waypointIndex].position : Vector3.zero;

			charMove.Move(Vector3.zero,false,lookPos,false);
		}
	}
}
