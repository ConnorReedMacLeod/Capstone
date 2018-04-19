﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:: Consider if not making this an observer is okay
[RequireComponent (typeof(MouseHandler))]
public class ViewAction : MonoBehaviour {

	public int id;                              //The action's unique identifier
	public Action mod;                      		//The action's model
	public MouseHandler mousehandler;
	public ViewActionWheel viewActionWheel;     //The action's ActionWheel segment's view

    //Sets the ActionWheel segment's material
	public void SetActionMaterial (string _sName){
		string sMatPath = "Materials/Actions/matAction" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;
		GetComponent<Renderer> ().material = matChr;
	}

    //Let the Action button know which character and id it's representing
	public void SetModel (Action _mod){
		mod = _mod;
	}

	public void Start(){
		InitMouseHandler ();
	}

	public void InitMouseHandler(){
		mousehandler = GetComponent<MouseHandler> ();
		mousehandler.SetOwner (this);

		mousehandler.SetNtfClick (Notification.ClickAct);//TODO:: Remove this
		mousehandler.SetNtfStartHover (Notification.ActStartHover);
		mousehandler.SetNtfStopHover (Notification.ActStopHover);
	}

    //Notifies application when the action's ActionWheel segment is clicked
    public void OnMouseDown(){
		//Controller.Get().NotifyObs(Notification.ClickAct, this, id);
	}	

	public void OnMouseEnter(){
		//Controller.Get ().NotifyObs (Notification.ActStartHover, this, id);
	}

	public void OnMouseExit(){
		//Controller.Get ().NotifyObs (Notification.ActStopHover, this, id);
	}
}
