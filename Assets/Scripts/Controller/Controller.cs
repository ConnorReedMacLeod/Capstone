using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These should control actions and logical flow of the game
// map actions taken with the view to changes in the model
// Can maintain information about game state

// The container class for Controllers
// Control flow:
// Events are channeled into this container which then distributes
// among the contained controllers

[RequireComponent (typeof(ContTarget))]
[RequireComponent (typeof(ContTimeline))]
[RequireComponent (typeof(ContMana))]
[RequireComponent (typeof(ContInfo))]
[RequireComponent (typeof(ContArena))]
public class Controller : Subject{


	ContTarget contTarget;
	ContTimeline contTimeline;
	ContMana contMana;
	ContInfo contInfo;
	ContArena contArena;
	ContGlobalInput contGlobalInput;

	public static Controller Get (){
		GameObject go = GameObject.FindGameObjectWithTag ("Controller");
		if (go == null) {
			Debug.LogError ("ERROR! NO OBJECT HAS A CONTROLLER TAG!");
		}
		Controller instance = go.GetComponent<Controller> ();
		if (instance == null) {
			Debug.LogError ("ERROR! CONTROLLER TAGGED OBJECT DOES NOT HAVE A CONTROLLER COMPONENT!");
		}
		return instance;
	}

	public void Start () {
		gameObject.tag = "Controller";

		// Find all necessary controllers and register them as our observers
		contTarget = GetComponent<ContTarget> ();
		Subscribe (contTarget);

		contTimeline = GetComponent<ContTimeline> ();
		Subscribe (contTimeline);

		contMana = GetComponent<ContMana> ();
		Subscribe (contMana);

		contInfo = GetComponent<ContInfo> ();
		Subscribe (contInfo);

        contArena = GetComponent<ContArena> ();
		Subscribe (contArena);

		contGlobalInput = GetComponent<ContGlobalInput> ();
		Subscribe (contGlobalInput);
	}

}