using UnityEngine;
using System.Collections;

public class CastScript : MonoBehaviour {

	//Cast objects movement
	void Update () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 2);
	}
}
