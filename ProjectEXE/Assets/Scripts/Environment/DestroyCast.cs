using UnityEngine;
using System.Collections;

public class DestroyCast : MonoBehaviour {

	//Cast destroyer object
	void OnCollisionEnter(Collision col){
		if (col.collider.gameObject.tag == "MovingPlatform") {
			Destroy(col.gameObject);
		}
	}
}
