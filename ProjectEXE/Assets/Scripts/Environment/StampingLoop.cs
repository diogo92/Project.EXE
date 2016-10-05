using UnityEngine;
using System.Collections;

public class StampingLoop : MonoBehaviour {

	//Stamping machines' behaviour

	bool newClip = true;

	void Update () {
		if (newClip) {
			StartCoroutine ("WaitClipLoop");
			newClip=false;
		}
	}

	public IEnumerator WaitClipLoop(){
		yield return new WaitForSeconds (GetComponent<Animation> ().clip.length);
		GetComponent<Animation> ().Stop ();
		yield return new WaitForSeconds (2);
		newClip = true;
		GetComponent<Animation> ().Play ();
	}
}
