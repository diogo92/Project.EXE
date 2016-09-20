using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
	public GameObject OptionsMenuUI;
	public bool options = false;
	public Text volume;

	public GameObject camera;

	void Start(){
		AudioListener.volume = 5;
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if(!options){
				Screen.lockCursor =false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				camera.GetComponent<FreeCameraLook>().pause = true;
				options=true;
				OptionsMenuUI.SetActive(true);
				Time.timeScale=0;
			}
			else{
				DisableMenu();
			}

		}
		if (options) {
			volume.text=AudioListener.volume.ToString();
		}
	}

	public void DisableMenu(){
		camera.GetComponent<FreeCameraLook>().pause = false;
		Screen.lockCursor = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		options=false;
		OptionsMenuUI.SetActive(false);
		Time.timeScale=1;
	}

	public void VolumeUp(){
		if (AudioListener.volume < 10)
			AudioListener.volume++;
	}

	public void VolumeDown(){
		if (AudioListener.volume > 0)
			AudioListener.volume--;
	}

	public void ReturnToMainMenu(){
		DisableMenu ();
		Application.LoadLevel (0);
	}

	public void ResetLevel(){
		DisableMenu ();
		PlayerPrefs.DeleteKey ("X");
		PlayerPrefs.DeleteKey ("Y");
		PlayerPrefs.DeleteKey ("Z");
		Application.LoadLevel (Application.loadedLevel);
	}

	public void ReturnToCheckpoint(){
		DisableMenu ();
		Application.LoadLevel (Application.loadedLevel);
	}


}
