using UnityEngine;
using System.Collections;

public class SightScript : MonoBehaviour {

	//Enemy sight handler

	CharacterStats charStat;
	EnemyAI enAI;

	void Start () {
		charStat = GetComponentInParent<CharacterStats> ();
		enAI = GetComponentInParent<EnemyAI> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<CharacterStats> ()) {
			if(other.GetComponent<CharacterStats>().Id != charStat.Id){
				if(!enAI.Enemies.Contains(other.gameObject)){
					enAI.Enemies.Add (other.gameObject);
				}
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (enAI.Enemies.Contains (other.gameObject)) {
			enAI.Enemies.Remove(other.gameObject);
		}
	}
}
