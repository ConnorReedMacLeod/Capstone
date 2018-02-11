using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAction : Observer {

	public int id;

	public ViewActionWheel viewActionWheel;

	public void SetActionMaterial (string _sName){
		string sMatPath = "Materials/Actions/matAction" + _sName;
		Debug.Log (sMatPath);
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponent<Renderer> ().material = matChr;
	}

	public ViewAction(ViewActionWheel _viewActionWheel, int _id){
		viewActionWheel = _viewActionWheel;
		id = _id;
	}
}
