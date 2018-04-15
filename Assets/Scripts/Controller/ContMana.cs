using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContMana : Observer{
	public void Start () {
		InitBindings ();
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch(eventType){
		case Notification.ManaAddPhysical: 
			AddPhysical();
			break;
		case Notification.ManaAddAllPhysical: 
			AddAllPhysical();
			break;
		case Notification.ManaRemovePhysical: 
			RemovePhysical();
			break;
		case Notification.ManaRemoveAllPhysical: 
			RemoveAllPhysical();
			break;

		case Notification.ManaAddMental: 
			AddMental();
			break;
		case Notification.ManaAddAllMental: 
			AddAllMental();
			break;
		case Notification.ManaRemoveMental: 
			RemoveMental();
			break;
		case Notification.ManaRemoveAllMental: 
			RemoveAllMental();
			break;

		case Notification.ManaAddEnergy: 
			AddEnergy();
			break;
		case Notification.ManaAddAllEnergy: 
			AddAllEnergy();
			break;
		case Notification.ManaRemoveEnergy: 
			RemoveEnergy();
			break;
		case Notification.ManaRemoveAllEnergy: 
			RemoveAllEnergy();
			break;

		case Notification.ManaAddBlood: 
			AddBlood();
			break;
		case Notification.ManaAddAllBlood: 
			AddAllBlood();
			break;
		case Notification.ManaRemoveBlood: 
			RemoveBlood();
			break;
		case Notification.ManaRemoveAllBlood: 
			RemoveAllBlood();
			break;

		case Notification.ManaAddAll:
			AddAll();
			break;
		case Notification.ManaRemoveAll:
			RemoveAll();
			break;

		default:
			break;
		}
	}

	public void InitBindings(){
		KeyBindings.SetBinding (Notification.ManaAddPhysical, KeyCode.Q);
		KeyBindings.SetBinding (Notification.ManaAddAllPhysical, KeyCode.Q, KeyCode.LeftShift);
		KeyBindings.SetBinding (Notification.ManaRemovePhysical, KeyCode.A);
		KeyBindings.SetBinding (Notification.ManaRemoveAllPhysical, KeyCode.A, KeyCode.LeftShift);

		KeyBindings.SetBinding (Notification.ManaAddMental, KeyCode.W);
		KeyBindings.SetBinding (Notification.ManaAddAllMental, KeyCode.W, KeyCode.LeftShift);
		KeyBindings.SetBinding (Notification.ManaRemoveMental, KeyCode.S);
		KeyBindings.SetBinding (Notification.ManaRemoveAllMental, KeyCode.S, KeyCode.LeftShift);

		KeyBindings.SetBinding (Notification.ManaAddEnergy, KeyCode.E);
		KeyBindings.SetBinding (Notification.ManaAddAllEnergy, KeyCode.E, KeyCode.LeftShift);
		KeyBindings.SetBinding (Notification.ManaRemoveEnergy, KeyCode.D);
		KeyBindings.SetBinding (Notification.ManaRemoveAllEnergy, KeyCode.D, KeyCode.LeftShift);

		KeyBindings.SetBinding (Notification.ManaAddBlood, KeyCode.R);
		KeyBindings.SetBinding (Notification.ManaAddAllBlood, KeyCode.R, KeyCode.LeftShift);
		KeyBindings.SetBinding (Notification.ManaRemoveBlood, KeyCode.F);
		KeyBindings.SetBinding (Notification.ManaRemoveAllBlood, KeyCode.F, KeyCode.LeftShift);

		KeyBindings.SetBinding (Notification.ManaAddAll, KeyCode.T);
		KeyBindings.SetBinding (Notification.ManaRemoveAll, KeyCode.G);
	}

	//CONSIDER:: Maybe abstract these to be created by a factory
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
}