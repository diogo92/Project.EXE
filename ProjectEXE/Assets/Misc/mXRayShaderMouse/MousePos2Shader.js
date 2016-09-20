// update object position to shader v1.0 - mgear - http://unitycoder.com/blog

#pragma strict

private var radius:float=2;


function Update () 
{

	// get mouse pos
	var ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
	var hit : RaycastHit;
	if (Physics.Raycast (ray, hit, Mathf.Infinity)) 
	{
		GetComponent.<Renderer>().material.SetVector("_ObjPos", Vector4(hit.point.x,hit.point.y,hit.point.z,0));

		
	}

	
	// box rotation for testing..
	if (Input.GetKey ("a"))
	{
		transform.Rotate(Vector3(0,30,0) * Time.deltaTime);
	}
	if (Input.GetKey ("d"))
	{
		transform.Rotate(Vector3(0,-30,0) * Time.deltaTime);
	}
	
	// mousewheel for radius
	if (Input.GetAxis("Mouse ScrollWheel")!=0)
	{
		radius +=Input.GetAxis("Mouse ScrollWheel")*0.8;
		GetComponent.<Renderer>().material.SetFloat( "_Radius", radius);
	}
}