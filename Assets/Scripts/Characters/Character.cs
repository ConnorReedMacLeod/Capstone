using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public Material matChr;
	public string sName;

	void InitMaterial(){
		string sMatPath = "Materials/Characters/mat" + sName;
		matChr = Resources.Load(sMatPath, typeof(Material)) as Material;
		GetComponent<Renderer> ().material = matChr;
	}

	void Initialize(){
		InitMaterial ();
	}

	public Character(){
		
	}

	// Use this for initialization
	public void Start () {
		Debug.Log ("Base Character Starting");
		Initialize ();
	}
	
	// Update is called once per frame
	public void Update () {
		
	}
}
