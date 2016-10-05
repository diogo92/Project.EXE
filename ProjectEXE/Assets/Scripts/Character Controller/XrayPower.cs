using UnityEngine;
using System.Collections;

public class XrayPower : MonoBehaviour
{
	//Xray power behaviour
	public Material[] materials;
	public float radius = 5f;
	GameObject lastHit;

	//Raycast to mouse position, if an invisible object is hit, it turns visible
	public void XRayPower ()
	{
		// get mouse pos
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		RaycastHit hit;
		//int mask = ~(1 << 9);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity/*,mask*/)) {

			if (hit.transform.gameObject.GetComponent<Renderer> () != null) {
				if (lastHit != null) {
					if (hit.transform.gameObject.name != lastHit.name) {
						if (lastHit.transform.gameObject.tag == "Invisible") {
							lastHit.GetComponent<Renderer> ().material = materials [0];
						} else {
						}
						if (hit.transform.gameObject.tag == "Invisible"){
							hit.transform.gameObject.GetComponent<Renderer> ().material = materials [1];

						}
						lastHit = hit.transform.gameObject;
					}	
				} else {
					if (hit.transform.gameObject.tag == "Invisible"){
						hit.transform.gameObject.GetComponent<Renderer> ().material = materials [1];
						lastHit = hit.transform.gameObject;
					}
				}
			}
		}
				
	}

	public void StopXRay ()
	{
		if (lastHit != null) {
			if (lastHit.transform.gameObject.tag == "Invisible") {
				lastHit.GetComponent<Renderer> ().material = materials [0];
			} else {
				lastHit.GetComponent<Renderer> ().material = materials [1];
			}
			lastHit = null;
		}
	}
}
