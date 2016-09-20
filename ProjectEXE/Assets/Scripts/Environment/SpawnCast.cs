using UnityEngine;
using System.Collections;

public class SpawnCast : MonoBehaviour {

	public GameObject castPrefab;
	float timepassed = 0f;

	void Update () {
		timepassed += Time.deltaTime;
		if (timepassed >= 4) {
			timepassed = 0;
			GameObject go = (GameObject)Instantiate((Object) castPrefab, transform.position, Quaternion.identity);
		}
	}
}
