using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Figure out if there's a way to use namespaces so that you don't always have to do 
//       KeyBindings.AddBinding every time

//NOTE:: This currently assumes that a keybind can only be bound to one action
//       Consider if this is a valid assumption to make

public class KeyBindings : MonoBehaviour{

	public Dictionary<Subject.FnCallback, KeyBind> dictEventToBind = new Dictionary<Subject.FnCallback, KeyBind>();
	public Dictionary<KeyBind, Subject.FnCallback> dictBindToEvent = new Dictionary<KeyBind, Subject.FnCallback>();

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

    public static KeyBindings inst;

    public void Awake() {

        Debug.Log("inst is " + inst);

        if (inst != null) {
            Debug.Log("Should destroy this inst");
            Destroy(this.gameObject);
            return;
        }

        inst = this;

        DontDestroyOnLoad(inst.gameObject);

        Debug.Log("Resetting keybind lists");

        dictEventToBind = new Dictionary<Subject.FnCallback, KeyBind>();
        dictBindToEvent = new Dictionary<KeyBind, Subject.FnCallback>();
    }

    public static KeyBindings Get(){
		return inst;
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

			foreach (KeyValuePair<KeyBind, Subject.FnCallback> otherBind in inst.dictBindToEvent){
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

        }
	}


	void Update () {

		foreach (KeyValuePair<KeyBind, Subject.FnCallback> bind in dictBindToEvent) {
			if (BindingUsed (bind.Key)) {
                //If a binding is satisfied, then activate it's function

                bind.Value(null);//Then call the callback function assigned to this binding
				break; //Only execute one action
			}
		}
	}

	public static void SetBinding(Subject.FnCallback fnCallback, KeyCode _key, KeyCode _keyModifier = KeyCode.None){

        //If this event is already registered to some key binding
		if (inst.dictEventToBind.ContainsKey(fnCallback)) {
            //Then Find that binding
			KeyBind curBind = inst.dictEventToBind [fnCallback];

            //And unregister the event currently linked to that binding
            //(The one we just consumed)
            inst.dictBindToEvent.Remove (curBind);
		}

        //Create the new desired binding
		KeyBind newBind = new KeyBind (_key, _keyModifier);

		if (inst.dictBindToEvent.ContainsKey (newBind)) {
			//If this binding is already linked to an event

            //Fetch the event currently linked to that binding
			Subject.FnCallback curCallback = inst.dictBindToEvent[newBind];
            //And unbind it
            inst.dictEventToBind.Remove (curCallback);
		}

        inst.dictEventToBind [fnCallback] = newBind;
        inst.dictBindToEvent [newBind] = fnCallback;
	}
}