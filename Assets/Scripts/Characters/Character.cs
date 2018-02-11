using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Subject {

	public enum CHARTYPE {
		LANCER, KATARA, SKELCOWBOY
	};

	Arena arena;

	public bool bSelected;

	public int id;
	public int idOwner;

	public string sName;

	// TODO:: Reconsider making this pos range from -0.5 to 0.5
	//        it's nice for avoiding some standard units of measurement,
	//        but it means there's only oen map size
	public Vector3 pos;

	public float fwidth;
	public float fheight;

	// Just to make it nicer to type
	public void SetPosition(float _fX, float _fY){
		SetPosition (new Vector3 (_fX, _fY, 0));
	}

	public void SetPosition(Vector3 _pos){
		pos = _pos;

		NotifyObs ();
	}

	public void Select(){
		bSelected = true;
		NotifyObs ();
	}

	public void Deselect (){
		bSelected = false;
		NotifyObs ();
	}

	public Character(int _idOwner, int _id){
		idOwner = _idOwner;
		id = _id;

		bSelected = false;

	}
		
}
