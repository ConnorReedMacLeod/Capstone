using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContMana : Observer {

	public delegate void fnManaAction();

	public Dictionary<fnManaAction, KeyBind> dictFnToBind;
	public Dictionary<KeyBind, fnManaAction> dictBindToFn;

	public struct KeyBind{
		public KeyCode keyPress;
		public KeyCode keyModifier;

		public KeyBind(KeyCode _keyPress, KeyCode _keyModifier){
			keyPress = _keyPress;
			keyModifier = _keyModifier;
		}
	}

	public bool BindingUsed(KeyBind bind){
		if (bind.keyModifier != KeyCode.None && !Input.GetKey (bind.keyModifier)) {
			//If there is a key that needs to be held down, then
			//if it's not held down the binding isn't satisfied
			return false;
		}
		return (Input.GetKeyDown(bind.keyPress));
	}

	public void InitBindings(){
		SetBinding (AddPhysical, KeyCode.Q);
		SetBinding (AddAllPhysical, KeyCode.Q, KeyCode.LeftShift);
		SetBinding (RemovePhysical, KeyCode.A);
		SetBinding (RemoveAllPhysical, KeyCode.A, KeyCode.LeftShift);

		SetBinding (AddMental, KeyCode.W);
		SetBinding (AddAllMental, KeyCode.W, KeyCode.LeftShift);
		SetBinding (RemoveMental, KeyCode.S);
		SetBinding (RemoveAllMental, KeyCode.S, KeyCode.LeftShift);

		SetBinding (AddEnergy, KeyCode.E);
		SetBinding (AddAllEnergy, KeyCode.E, KeyCode.LeftShift);
		SetBinding (RemoveEnergy, KeyCode.D);
		SetBinding (RemoveAllEnergy, KeyCode.D, KeyCode.LeftShift);

		SetBinding (AddBlood, KeyCode.R);
		SetBinding (AddAllBlood, KeyCode.R, KeyCode.LeftShift);
		SetBinding (RemoveBlood, KeyCode.F);
		SetBinding (RemoveAllBlood, KeyCode.F, KeyCode.LeftShift);

		SetBinding (AddAll, KeyCode.T);
		SetBinding (RemoveAll, KeyCode.G);
	}

	//TODO:: Maybe abstract these to be created by a factory
	public void AddPhysical(){
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.PHYSICAL);
	}
	public void AddAllPhysical(){
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.PHYSICAL];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.PHYSICAL, totalMana);
	}
	public void RemovePhysical(){
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.PHYSICAL);
	}
	public void RemoveAllPhysical(){
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.PHYSICAL];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.PHYSICAL, totalPool);
	}


	public void AddMental(){
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.MENTAL);
	}
	public void AddAllMental(){
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.MENTAL];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.MENTAL, totalMana);
	}
	public void RemoveMental(){
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.MENTAL);
	}
	public void RemoveAllMental(){
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.MENTAL];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.MENTAL, totalPool);
	}


	public void AddEnergy(){
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.ENERGY);
	}

	public void AddAllEnergy(){
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.ENERGY];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.ENERGY, totalMana);
	}
	public void RemoveEnergy(){
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.ENERGY);
	}
	public void RemoveAllEnergy(){
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.ENERGY];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.ENERGY, totalPool);
	}


	public void AddBlood(){
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.BLOOD);
	}

	public void AddAllBlood(){
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.BLOOD];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.BLOOD, totalMana);
	}
	public void RemoveBlood(){
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.BLOOD);
	}
	public void RemoveAllBlood(){
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.BLOOD];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.BLOOD, totalPool);
	}

	public void AddAll(){
		AddAllPhysical ();
		AddAllMental ();
		AddAllEnergy ();
		AddAllBlood ();
	}

	public void RemoveAll(){
		RemoveAllPhysical ();
		RemoveAllMental ();
		RemoveAllEnergy ();
		RemoveAllBlood ();
	}

	public void Start () {
		dictFnToBind = new Dictionary<fnManaAction, KeyBind> ();
		dictBindToFn = new Dictionary<KeyBind, fnManaAction> ();

		InitBindings ();
	}


	void Update () {

		foreach (KeyValuePair<KeyBind, fnManaAction> bind in dictBindToFn) {
			if (BindingUsed (bind.Key)) {
				//If a binding is satisfied, then activate it's function

				bind.Value ();
			}
		}
	}
		
	public void SetBinding(fnManaAction fnAction, KeyCode _key, KeyCode _keyModifier = KeyCode.None){

		if (dictFnToBind.ContainsKey(fnAction)) {
			KeyBind curBind = dictFnToBind [fnAction];

			//Unbind the currently used key
			dictBindToFn.Remove (curBind);
		}
		KeyBind newBind = new KeyBind (_key, _keyModifier);

		if (dictBindToFn.ContainsKey (newBind)) {
			// If the new binding is used for something else, then
			// unbind the other thing
			fnManaAction curAction = dictBindToFn[newBind];
			dictFnToBind.Remove (curAction);
		}

		dictFnToBind [fnAction] = newBind;
		dictBindToFn [newBind] = fnAction;
	}
}
