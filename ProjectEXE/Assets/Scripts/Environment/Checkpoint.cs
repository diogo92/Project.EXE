using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Checkpoint : MonoBehaviour {

	public GameObject text;
	public bool activated = false;
	public static GameObject[] CheckPointsList;
	// Use this for initialization
	void Start () {
		CheckPointsList = GameObject.FindGameObjectsWithTag("CheckPoint");
	}
	
	public static Vector3 GetActiveCheckPointPosition()
	{
		// If player die without activate any checkpoint, we will return a default position
		Vector3 result = new Vector3(0, 0, 0);
		
		if (CheckPointsList != null)
		{
			foreach (GameObject cp in CheckPointsList)
			{
				// We search the activated checkpoint to get its position
				if (cp.GetComponent<Checkpoint>().activated)
				{
					result = cp.transform.position;
					break;
				}
			}
		}
		
		return result;
	}

	private void ActivateCheckPoint()
	{
		// We deactive all checkpoints in the scene
		foreach (GameObject cp in CheckPointsList)
		{
			cp.GetComponent<Checkpoint>().activated = false;
		}
		
		// We activate the current checkpoint
		activated = true;
		StartCoroutine ("showtext");
		PlayerPrefs.SetFloat ("X", transform.position.x);
		PlayerPrefs.SetFloat ("Y", transform.position.y);
		PlayerPrefs.SetFloat ("Z", transform.position.z);



	}

	public IEnumerator showtext(){
		text.SetActive (true) ;
		yield return new WaitForSeconds (2);
		text.SetActive (false) ;
	}

	void OnTriggerEnter(Collider other)
	{
		// If the player passes through the checkpoint, we activate it
		if (other.tag == "Player" && !activated)
		{
			ActivateCheckPoint();
		}
	}
}
