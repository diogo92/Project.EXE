using UnityEngine;
using System.Collections;

public class StampingLoop : MonoBehaviour {

	bool newClip = true;
	// Update is called once per frame
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
