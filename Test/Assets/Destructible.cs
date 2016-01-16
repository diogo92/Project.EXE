using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

	public bool destruct;

	public GameObject[] debris;
	// Use this for initialization
	void Start () {
		if (debris.Length > 0) {
			foreach (GameObject go in debris) {
				go.SetActive (false);
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if (destruct) {
			if(debris.Length > 0){
				foreach(GameObject go in debris){
					go.SetActive(true);
					go.transform.parent = null;
				}
			}

			Destroy(gameObject);
		}
	}
}
