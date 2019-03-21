using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO :: Should be able to convert generally shared controllers
//        to static instances.  However, for something like ContMana,
//        each player will need their own instance to ensure
//        player input only works for their instance of mana


// These should control actions and logical flow of the game
// map actions taken with the view to changes in the model
// Can maintain information about game state

// The container class for Controllers
// Control flow:
// This will subscribe to every subject that exists
// so that if any other observers need to know if some event
// happend anywhere (say across all character selections), they 
// can subscribe to this mega-subject class

[RequireComponent (typeof(ContAbilitySelection))]
[RequireComponent (typeof(ContMana))]
[RequireComponent (typeof(ContInfo))]
[RequireComponent (typeof(ContArena))]
public class Controller : MonoBehaviour{

    //TODO:: Make all of these controllers static instances, so a controller object isn't needed
    
    public ContAbilitySelection contAbilitySelection;
    public ContMana contMana;
    public ContInfo contInfo;
    public ContArena contArena;

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
        contAbilitySelection = GetComponent<ContAbilitySelection> ();
		//Subscribe (contTarget);

		contMana = GetComponent<ContMana> ();
		//Subscribe (contMana);

		contInfo = GetComponent<ContInfo> ();
		//Subscribe (contInfo);

        contArena = GetComponent<ContArena> ();
		//Subscribe (contArena);
	}

}