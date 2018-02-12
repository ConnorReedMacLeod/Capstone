using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAction : Observer {

	public int id;
	public Character mod;

	public ViewActionWheel viewActionWheel;

	public void SetActionMaterial (string _sName){
		string sMatPath = "Materials/Actions/matAction" + _sName;

		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponent<Renderer> ().material = matChr;
	}

	public void SetModel (Character _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

	public void OnMouseDown(){
		app.Notify (Notification.ClickAct, this, id);
	}

	public ViewAction(ViewActionWheel _viewActionWheel, int _id){
		viewActionWheel = _viewActionWheel;
		id = _id;
	}
}
