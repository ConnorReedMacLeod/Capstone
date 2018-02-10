using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class of all elements
public class Element : MonoBehaviour {

	// Gives access to the application
	public App app { get { return GameObject.FindObjectOfType<App> (); } }
}


public class App : MonoBehaviour {

	public Model model;
	public View view;
	public Controller cont;

	// Use this for initialization
	void Start () {
		
	}

}
