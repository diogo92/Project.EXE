using UnityEngine;
using System.Collections;

public class DestroyCast : MonoBehaviour {

	void OnCollisionEnter(Collision col){
		if (col.collider.gameObject.tag == "MovingPlatform") {
			Destroy(col.gameObject);
		}
	}
}
