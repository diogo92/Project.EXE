using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public void StartGame(){
		Application.LoadLevel ("test");
	}

	public void ExitGame(){
		Application.Quit ();
	}
}
