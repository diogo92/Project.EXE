using UnityEngine;
using System.Collections;

public class CharacterSoundController : MonoBehaviour {

	bool dead = false;
	AudioSource audioSource;
	public AudioSource xraySource;
	public AudioClip walk;
	public AudioClip run;
	public AudioClip jump;
	public AudioClip die;
	public AudioClip punch;
	public AudioClip xray;
	public AudioClip pickup;

	public void Start(){
		audioSource = GetComponent<AudioSource> ();
		xraySource.clip = xray;
	}

	public void PlayWalk(bool isRunning){
		if (!audioSource.isPlaying) {
			if (!isRunning) {
				audioSource.Stop ();
				audioSource.clip = walk;
				audioSource.Play ();
			} else {
				audioSource.Stop ();
				audioSource.clip = run;
				audioSource.Play ();
			}
		}
	}

	public void PlayJump(){
		audioSource.Stop ();
		audioSource.clip = jump;
		audioSource.Play ();
	}

	public void PlayPunch(){
		audioSource.Stop ();
		audioSource.clip = punch;
		audioSource.Play ();
	}

	public void PlayXray(){
		if(!xraySource.isPlaying)
			xraySource.Play ();
	}
	public void StopXray(){
		xraySource.Stop ();
	}

	public void PlayDie(){
		if (!dead) {
			dead = true;
			audioSource.Stop ();
			audioSource.clip = die;
			audioSource.Play ();
		} 
	}

	public void PlayPickUp(){
		audioSource.Stop ();
		audioSource.clip = pickup;
		audioSource.Play ();
	}
}
