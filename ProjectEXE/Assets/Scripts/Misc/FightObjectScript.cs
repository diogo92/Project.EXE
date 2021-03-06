using UnityEngine;
using System.Collections;

public class FightObjectScript : MonoBehaviour {

	//Checks collision between an enemy and the object
	public ArrayList enemies;
	void Start(){
		enemies = new ArrayList ();
	}

	void OnTriggerEnter (Collider other){
		if (other.transform.parent) {
			if (other.transform.parent.GetComponent<CharacterStats>() && other.gameObject.transform.parent.tag != transform.parent.tag && !enemies.Contains (other)) {
				if (other.GetType () == typeof(BoxCollider)) {
					enemies.Add (other);
				}
			}
		}
	}

	void OnTriggerExit (Collider other){

		if (other.transform.parent) {
			if (enemies.Contains (other)) {
				enemies.Remove (other);
			}
		}
	}
}
