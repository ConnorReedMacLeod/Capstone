using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChr : Observer {

	public Character mod;

	public ViewArena viewArena;

	//keep some local variables to keep track of things that have changed
	bool lastbSelected;
	Vector3 lastPos;


	const int indexCharBorder = 0;
	const int indexCharPortrait = 1;

	//undoes the scaling of the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}

	void setPortrait(string _sName){
		string sMatPath = "Materials/Characters/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharPortrait].material = matChr;
	}

	void setBorder(string _sName){
		string sMatPath = "Materials/mat" + _sName;
		Material matBorder = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharBorder].material = matBorder;
	}

	public void setModel(Character _mod){
		mod = _mod;
		mod.Subscribe (this);

		// Set the portrait to be this new character
		setPortrait (mod.sName);
	}

	void UpdatePos(){
		if (lastPos != mod.pos) {
			lastPos = mod.pos;

			//TODO:: Will probably update this to be some animation
			transform.localPosition = lastPos;
		}
	}

	void UpdateStatus(){
		if (lastbSelected != mod.bSelected) {
			lastbSelected = mod.bSelected;
			if (lastbSelected) {
				setBorder ("ChrBorderSelected");
			} else {
				setBorder ("ChrBorder");
			}

		}
	}

	override public void UpdateObs(){
		// Detect if the position has been changed
		UpdatePos();

		// Detect if selected status has changed
		UpdateStatus();
	}

	public void OnMouseDown(){
		app.Notify (Notification.ClickChr, this);
	}

	public void Start(){
		Unscale ();
	}
}
