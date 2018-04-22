using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Figure out if there's a way to use namespaces so that you don't always have to do 
//       KeyBindings.AddBinding every time


public class KeyBindings : MonoBehaviour{

	public static KeyBindings instance;

	public Dictionary<string, KeyBind> dictEventToBind;
	public Dictionary<KeyBind, string> dictBindToEvent;

	public bool bStarted;

	public struct KeyBind{
		public KeyCode keyPress;
		public KeyCode keyModifier;

		public KeyBind(KeyCode _keyPress, KeyCode _keyModifier){
			//TODO: Handle invalid Press and Modifier selections
			keyPress = _keyPress;
			keyModifier = _keyModifier;
		}
	}

	public static KeyBindings Get(){
		if(instance == null){
			GameObject go = GameObject.FindGameObjectWithTag("Controller");
			if(go == null){
				Debug.LogError("ERROR!  NO OBJECT WITH 'controller' TAG!");
			}
			instance = go.GetComponent<KeyBindings>();
			if (instance == null){
				Debug.LogError("ERROR!  CONTROLLER HAS NO KEYBINDINGS COMPONENT!");
			}
			instance.Start ();
		}
		return instance;
	}

	public static bool BindingUsed(KeyBind bind){
		if (bind.keyModifier != KeyCode.None && !Input.GetKey (bind.keyModifier)) {
			//If there is a key that needs to be held down, then
			//if it's not held down the binding isn't satisfied
			//TODO:: Enable weirdos who press right shift to do stuff
			return false;
		}else if (Input.GetKeyDown(bind.keyPress) && bind.keyModifier == KeyCode.None){
			//If there's no modifier to this binding, check if there's a 
			//binding for this key with a modifier that is satisfied

			foreach (KeyValuePair<KeyBind, string> otherBind in Get().dictBindToEvent){
				if(otherBind.Key.keyPress == bind.keyPress &&
					otherBind.Key.keyModifier != KeyCode.None &&
					Input.GetKey(otherBind.Key.keyModifier)){

					//Since a more complicated binding is satisfied, yield to it
					return false;
				}

			}

		}
		return (Input.GetKeyDown(bind.keyPress));
	}

	public void Start () {
		if(bStarted == false){
			bStarted = true;

			dictEventToBind = new Dictionary<string, KeyBind> ();
			dictBindToEvent = new Dictionary<KeyBind, string> ();

			Get();//To initialize the static instance
		}
	}


	void Update () {
		Start ();

		foreach (KeyValuePair<KeyBind, string> bind in dictBindToEvent) {
			if (BindingUsed (bind.Key)) {
				//If a binding is satisfied, then activate it's function

				Controller.Get().NotifyObs(bind.Value, null);
				break; //Only execute one action
			}
		}
	}

	public static void SetBinding(string sEvent, KeyCode _key, KeyCode _keyModifier = KeyCode.None){

		if (Get().dictEventToBind.ContainsKey(sEvent)) {
			KeyBind curBind = Get().dictEventToBind [sEvent];

			//Unbind the currently used key
			Get().dictBindToEvent.Remove (curBind);
		}
		KeyBind newBind = new KeyBind (_key, _keyModifier);

		if (Get().dictBindToEvent.ContainsKey (newBind)) {
			// If the new binding is used for something else, then
			// unbind the other thing
			string curEvent = Get().dictBindToEvent[newBind];
			Get().dictEventToBind.Remove (curEvent);
		}

		Get().dictEventToBind [sEvent] = newBind;
		Get().dictBindToEvent [newBind] = sEvent;
	}
}