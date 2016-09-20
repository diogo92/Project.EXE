using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
		StartCoroutine ("selfDestruct");
	}

	IEnumerator selfDestruct(){
		yield return new WaitForSeconds(0.3f);
		Destroy (gameObject);
	}
}
