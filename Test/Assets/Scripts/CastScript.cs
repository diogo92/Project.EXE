using UnityEngine;
using System.Collections;

public class CastScript : MonoBehaviour {

	void Update () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 2);
	}
}
