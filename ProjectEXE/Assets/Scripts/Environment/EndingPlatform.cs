using UnityEngine;
using System.Collections;

public class EndingPlatform : MonoBehaviour {

	//Ends the game when reaching the final platform
	public GameObject text;
	bool end = false;
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Player" && !end) {
			end = true;
			Screen.lockCursor = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Time.timeScale=1;
			StartCoroutine("endLevel");

		}

	}

	IEnumerator endLevel(){
		text.SetActive (true);
		yield return new WaitForSeconds(3);
		text.SetActive (false);
		Application.LoadLevel (0);
	}
}
