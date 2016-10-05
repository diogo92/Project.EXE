using UnityEngine;
using System.Collections;

public class PickableItem : MonoBehaviour {

	//Behaviour for pickable objects

	public bool CharacterInTrigger;
	public GameObject Owner;

	void Start(){
		GetComponent<SphereCollider> ().radius = 3;
	}
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<CharacterStats>()) {
			CharacterInTrigger = true;

			if(!Owner){
				Owner = other.gameObject;
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<CharacterStats> ()) {
			if(Owner != null){
				if(other.GetComponent<CharacterStats>().gameObject == Owner){
					CharacterInTrigger = false;
				}
			}
		}
	}
}
