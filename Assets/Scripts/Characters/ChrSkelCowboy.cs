using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrSkelCowboy : Character {

	ChrSkelCowboy():base(){

	}

	// Use this for initialization
	void Start () {
		sName = "SkelCowboy";
		Debug.Log ("I'm " + sName);
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
