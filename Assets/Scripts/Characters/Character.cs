﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Subject {

	public enum CHARTYPE {
		LANCER, KATARA, SKELCOWBOY
	};

	public enum STATESELECT{
		SELECTED, // selected, but not setting action targets
		TARGGETING, // selected and currently setting targets
		UNSELECTED // not selected
	}

	Arena arena;

	public int id;
	public int idOwner;

	public int nCurHealth;
	public int nMaxHealth;

	public string sName;

	// TODO:: Reconsider making this pos range from -0.5 to 0.5
	//        it's nice for avoiding some standard units of measurement,
	//        but it means there's only one map size
	public Vector3 pos;

	public float fwidth;
	public float fheight;

	public static int nActions = 8;
	public Action[] arActions;
	public int nUsingAction;
	public bool bSetAction; // is an action set for the Chr to execute

	public STATESELECT stateSelect;

	// Just to make it nicer to type
	public void SetPosition(float _fX, float _fY){
		SetPosition (new Vector3 (_fX, _fY, 0));
	}

	public void SetPosition(Vector3 _pos){
		pos = _pos;

		NotifyObs ();
	}

	public void Select(){
		stateSelect = STATESELECT.SELECTED;
		NotifyObs ();
	}

	public void Targetting(){
		stateSelect = STATESELECT.TARGGETING;
		NotifyObs ();
	}

	public void Deselect (){
		stateSelect = STATESELECT.UNSELECTED;
		NotifyObs ();
	}

	public void SetActions(){//TODO:: probably add some parameter for this at some point like an array of ids
		for (int i = 0; i < nActions; i+=2) {
			arActions [i] = new ActionFireball (this);
			arActions [i+1] = new ActionMove (this);
		}
	}

	public Character(int _idOwner, int _id){
		idOwner = _idOwner;
		id = _id;

		arActions = new Action[nActions];
		nUsingAction = -1;

		stateSelect = STATESELECT.UNSELECTED;

		SetActions ();//TODO:: move this somewhere else at some point
	}
		
}
