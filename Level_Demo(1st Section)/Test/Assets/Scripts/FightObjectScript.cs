using UnityEngine;
using System.Collections;

public class FightObjectScript : MonoBehaviour {

	public ArrayList enemies;
	void Start(){
		enemies = new ArrayList ();
	}

	void OnTriggerEnter (Collider other){
		if (other.transform.parent) {
			if (other.gameObject.transform.parent.tag == "Enemy" && !enemies.Contains (other)) {
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
