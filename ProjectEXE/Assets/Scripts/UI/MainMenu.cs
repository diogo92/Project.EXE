using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	//Main Menu behaviour, for showing the how to play screen and starting the game, while checking for previous checkpoints
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
