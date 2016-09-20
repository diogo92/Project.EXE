using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour {

	public float xspeed=0;
	public float yspeed=0;
	public float zspeed=0;

	public Vector3 endPos;
	Vector3 startPos;
	public bool translate = false;
	public float stopTime = 0;
	float timecounter = 0;


	public bool StartToEnd = true;
	bool counting = false;
	void Start(){
		startPos = transform.localPosition;
	}
	void Update () {
		if (translate) {
			if(Mathf.Round(transform.localPosition.x) == endPos.x)
				StartToEnd = false;
			if(Mathf.Round(transform.localPosition.x) == startPos.x)
				StartToEnd = true;
			if(StartToEnd)
				transform.localPosition = Vector3.Lerp(transform.localPosition,endPos,Time.deltaTime);
			else
				transform.localPosition = Vector3.Lerp(transform.localPosition,startPos,Time.deltaTime);


		} else {
			timecounter += Time.deltaTime;
			if (stopTime == 0)
				transform.Rotate (xspeed * Time.deltaTime, yspeed * Time.deltaTime, zspeed * Time.deltaTime);
			else {
				if (timecounter < 3) {
					transform.Rotate (xspeed * Time.deltaTime, yspeed * Time.deltaTime, zspeed * Time.deltaTime);
				} else {
					if (!counting)
						StartCoroutine ("RotateCooldown");
				}
			}
		}
	}

	IEnumerator RotateCooldown(){
		counting = true;
		transform.rotation = Quaternion.identity;
		yield return new WaitForSeconds (stopTime);
		timecounter = 0;
		counting = false;
	}
}
