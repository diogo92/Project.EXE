using UnityEngine;
using System.Collections;

public class XrayPower : MonoBehaviour
{

	public Material[] materials;
	public float radius = 5f;
	GameObject lastHit;
	// Update is called once per frame

	public void XRayPower ()
	{
		// get mouse pos
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		RaycastHit hit;
		//int mask = ~(1 << 9);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity/*,mask*/)) {
			//		renderer.material.SetVector("_ObjPos", Vector4(obj.position.x,obj.position.y,obj.position.z,0));

			if (hit.transform.gameObject.GetComponent<Renderer> () != null) {
				if (lastHit != null) {
					if (hit.transform.gameObject.name != lastHit.name) {
						if (lastHit.transform.gameObject.tag == "Invisible") {
							lastHit.GetComponent<Renderer> ().material = materials [0];
						} else {
						//	lastHit.GetComponent<Renderer> ().material = materials [1];
						//	lastHit.GetComponent<Renderer> ().material.shader = Shader.Find ("Standard");
						}
						hit.transform.gameObject.GetComponent<Renderer> ().material = materials [1];
						//hit.transform.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("mShaders/XRay1");
						lastHit = hit.transform.gameObject;
									
					}	
				} else {
					if (hit.transform.gameObject.tag == "Invisible"){
						hit.transform.gameObject.GetComponent<Renderer> ().material = materials [1];
						//hit.transform.gameObject.GetComponent<Renderer> ().material.shader = Shader.Find ("mShaders/XRay1");

						lastHit = hit.transform.gameObject;
					}
				}
				//if(lastHit.transform != null){
				//	if(hit.transform.gameObject != lastHit.transform.gameObject){
				/*lastHit.transform.GetComponent<Renderer>().material.SetFloat("_Radius",0f);
									lastHit = hit;*/
				//		lastHit.transform.GetComponent<Renderer>().material.shader = Shader.Find ("Standard");
				//		hit.transform.gameObject.GetComponent<Renderer>().material.shader=Shader.Find ("mShaders/XRay1");
				//		lastHit = hit;
				//}
				/*	}
							else{
								lastHit=hit;
								hit.transform.GetComponent<Renderer>().material.shader=Shader.Find ("mShaders/XRay1");
							}
							/*Vector3 local = hit.transform.InverseTransformPoint(hit.point);
							print(local);
							lastHit.transform.GetComponent<Renderer>().material.SetFloat("_Radius",2f);
							hit.transform.GetComponent<Renderer>().material.SetVector("_ObjPos", new Vector4(local.x,local.y,local.z,0));*/
			}
					
			// convert hit.point to our plane local coordinates, not sure how to do in shader.. IN.pos.. ??
			//		var hitlocal = transform.InverseTransformPoint(hit.point);
			//		renderer.material.SetVector("_ObjPos",Vector4(hitlocal.x,hitlocal.y,hitlocal.z,0));

		}
				
	}

	public void StopXRay ()
	{
		if (lastHit != null) {
			if (lastHit.transform.gameObject.tag == "Invisible") {
				lastHit.GetComponent<Renderer> ().material = materials [0];
			} else {
				lastHit.GetComponent<Renderer> ().material = materials [1];
			//	lastHit.GetComponent<Renderer> ().material.shader = Shader.Find ("Standard");
			}
			//lastHit.GetComponent<Renderer> ().material.shader = Shader.Find ("Standard");
			lastHit = null;
		}
	}
}
