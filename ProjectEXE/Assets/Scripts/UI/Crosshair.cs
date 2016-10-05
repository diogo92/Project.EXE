﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Crosshair : MonoBehaviour {
	//For reference only

	public string CrosshairName;

	public float defaultSpread = 15;
	public float maxSpread = 50;
	public float wiggleSpread = 50;
	public float wiggleSpreadMaxTimer = 60;

	[HideInInspector]
	public float currentSpread = 0;
	private float targetSpread = 0;
	private float spreadT = 0;
	private Quaternion defaultRotation;
	private bool isSpreadWorking = true;

	public float spreadSpeed = 0.2f;
	public float rotationSpeed = 0.5f;
	public bool allowRotating = true;

	private float rotationTimer = 0;
	private bool isRotating = false;
	public bool spreadWhileRotating = false;
	public float rotationSpread = 0;

	public bool allowSpread = true;

	private bool wiggle = false;
	private float wiggleTimer = 0;

	public CrosshairPart[] parts;


	void Start(){
		defaultRotation = transform.rotation;
		currentSpread = defaultSpread;
		ChangeCursorSpread (defaultSpread);
	}

	void Update(){
		if (isSpreadWorking) {
			spreadT += Time.deltaTime / spreadSpeed;

			float spreadValue = AccelDecelInterpl(currentSpread,targetSpread,spreadT);

			if(spreadT > 1){
				spreadValue = targetSpread;
				spreadT = 0;

				if(wiggle){
					if(wiggleTimer < wiggleSpreadMaxTimer){
						wiggleTimer += Time.deltaTime;
					}
					else{
						wiggleTimer = 0;
						wiggle = false;
						targetSpread = defaultSpread;
					}
				}
				else{
					isSpreadWorking = false;
				}

			}
			else{
				ChangeCursorSpread(defaultSpread);
			}
			currentSpread = spreadValue;
			ApplySpread();
		}
		if(isRotating){
			if(rotationTimer > 0){
				rotationTimer -= Time.deltaTime;
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z + (360 * Time.deltaTime * rotationSpeed));
				
			}
			else{
				isRotating = false;
				transform.rotation = defaultRotation;

				if(spreadWhileRotating){
					ChangeCursorSpread(defaultSpread);
				}
			}
		}
	}


	public void ApplySpread(){
		foreach (CrosshairPart im in parts) {
			im.image.rectTransform.anchoredPosition = im.up * currentSpread;
		}
	}

	public void WiggleCrosshair(){
		if (allowSpread) {
			ChangeCursorSpread(wiggleSpread);
			wiggle = true;
		}
	}

	public void ChangeCursorSpread(float value){
		if (allowSpread) {
			isSpreadWorking = true;
			targetSpread = value;
			spreadT = 0;
		}
	}

	public void rotateCursor(float seconds){
		if (allowRotating) {
			isRotating = true;
			rotationTimer = seconds;
			if(spreadWhileRotating){
				ChangeCursorSpread(rotationSpeed);
			}
		}
	}

	public static float AccelDecelInterpl(float start, float end, float t){
		float x = end - start;

		float newT = (Mathf.Cos((t+1) * Mathf.PI)/2) + 0.5f;

		x *= newT;

		float retVal = start + x;

		return retVal;
	}


	[Serializable]
	public class CrosshairPart{
		public Image image;
		public Vector2 up;
	}



}
