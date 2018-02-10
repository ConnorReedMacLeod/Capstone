using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Subject {

	public enum CHARTYPE {
		LANCER, KATARA, SKELCOWBOY
	};

	Arena arena;

	public int id;
	public int idOwner;

	public string sName;

	public float fX;
	public float fY;

	public void SetPosition(float _fX, float _fY){
		fX = _fX;
		fY = _fY;
	}

	public Character(int _idOwner, int _id){
		idOwner = _idOwner;
		id = _id;

	}

	public void Start (){
		SetPosition(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
	}
}
