using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {


	public GameObject htpScreen;

	public void StartGame(){
		PlayerPrefs.DeleteKey ("X");
		PlayerPrefs.DeleteKey ("Y");
		PlayerPrefs.DeleteKey ("Z");
		Application.LoadLevel ("Level 1");
	}

	public void ExitGame(){
		PlayerPrefs.DeleteKey ("X");
		PlayerPrefs.DeleteKey ("Y");
		PlayerPrefs.DeleteKey ("Z");
		Application.Quit ();
	}

	public void HTP(){
		htpScreen.SetActive (true);
	}

	public void Return(){
		htpScreen.SetActive (false);
	}

}
