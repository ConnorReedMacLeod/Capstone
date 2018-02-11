using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Subject {

	public enum CHARTYPE {
		LANCER, KATARA, SKELCOWBOY
	};

	Arena arena;

	public bool selected;

	public int id;
	public int idOwner;

	public string sName;

	public float fX;
	public float fY;
	public float fZ;

	public float fwidth;
	public float fheight;

	public void SetPosition(float _fX, float _fY){
		SetPosition (_fX, _fY, 0);
	}

	public void SetPosition(float _fX, float _fY, float _fZ){
		fX = _fX;
		fY = _fY;
		fZ = _fZ;

		NotifyObs ();
	}

	public void Select(){
		Debug.Log (sName + " has been selected");
		selected = true;
		NotifyObs ();
	}

	public void UnSelect (){
		Debug.Log (sName + " is unselected");
		selected = false;
		NotifyObs ();
	}

	public Character(int _idOwner, int _id){
		idOwner = _idOwner;
		id = _id;

		selected = false;

	}
		
}
