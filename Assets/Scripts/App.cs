using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Base class of all elements
public class Element : MonoBehaviour {

	// Gives access to the application
	public App app { get { return GameObject.FindObjectOfType<App> (); } }
}


public class App : MonoBehaviour {

	public Model model;
	public View view;

	//TODO: Eventually implement different controllers for
	//      selections, mana, timeline...
	public List<Controller> controllers;

	public void Notify(string eventType, Object target, params object[] args){

		foreach (Controller c in controllers) {
			c.OnNotification (eventType, target, args);
		}
	}

	// Use this for initialization
	void Start () {
		controllers = (GetComponentsInChildren<Controller> ()).ToList();

	}

}
