using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChrLancer : Character {

	ChrLancer():base(){

	}

	// Use this for initialization
	void Start () {
		sName = "Lancer";
		Debug.Log ("I'm " + sName);
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
