using UnityEngine;
using System.Collections;

public class MovingFloor : MonoBehaviour {

	//Treadmill behaviour used in the stamping machines' sections
	//for the floor to move automatically
	void Update () {
		for (int i = 0; i<transform.childCount; i++) {
			transform.GetChild(i).GetComponent<Transform> ().position = new Vector3 (transform.GetChild(i).GetComponent<Transform> ().position.x - 0.01f, transform.GetChild(i).GetComponent<Transform> ().position.y, transform.GetChild(i).GetComponent<Transform> ().position.z);
		}
	}
}
