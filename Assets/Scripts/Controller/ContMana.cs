using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContMana : MonoBehaviour{
	public void Start () {
		InitBindings ();
	}

	public void InitBindings(){
		KeyBindings.SetBinding (AddPhysical, KeyCode.Q);
		KeyBindings.SetBinding (AddAllPhysical, KeyCode.Q, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemovePhysical, KeyCode.A);
		KeyBindings.SetBinding (RemoveAllPhysical, KeyCode.A, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddMental, KeyCode.W);
		KeyBindings.SetBinding (AddAllMental, KeyCode.W, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveMental, KeyCode.S);
		KeyBindings.SetBinding (RemoveAllMental, KeyCode.S, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddEnergy, KeyCode.E);
		KeyBindings.SetBinding (AddAllEnergy, KeyCode.E, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveEnergy, KeyCode.D);
		KeyBindings.SetBinding (RemoveAllEnergy, KeyCode.D, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddBlood, KeyCode.R);
		KeyBindings.SetBinding (AddAllBlood, KeyCode.R, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveBlood, KeyCode.F);
		KeyBindings.SetBinding (RemoveAllBlood, KeyCode.F, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddAll, KeyCode.T);
		KeyBindings.SetBinding (RemoveAll, KeyCode.G);
	}

	//CONSIDER:: Maybe abstract these to be created by a factory
	public void AddPhysical(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.PHYSICAL);
	}
	public void AddAllPhysical(Object target, params object[] args) {
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.PHYSICAL];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.PHYSICAL, totalMana);
	}
	public void RemovePhysical(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.PHYSICAL);
	}
	public void RemoveAllPhysical(Object target, params object[] args) {
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.PHYSICAL];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.PHYSICAL, totalPool);
	}


	public void AddMental(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.MENTAL);
	}
	public void AddAllMental(Object target, params object[] args) {
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.MENTAL];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.MENTAL, totalMana);
	}
	public void RemoveMental(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.MENTAL);
	}
	public void RemoveAllMental(Object target, params object[] args) {
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.MENTAL];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.MENTAL, totalPool);
	}


	public void AddEnergy(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.ENERGY);
	}

	public void AddAllEnergy(Object target, params object[] args) {
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.ENERGY];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.ENERGY, totalMana);
	}
	public void RemoveEnergy(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.ENERGY);
	}
	public void RemoveAllEnergy(Object target, params object[] args) {
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.ENERGY];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.ENERGY, totalPool);
	}


	public void AddBlood(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.BLOOD);
	}

	public void AddAllBlood(Object target, params object[] args) {
		int totalMana = Match.Get ().GetLocalPlayer ().mana.arMana [(int)Mana.MANATYPE.BLOOD];
		Match.Get ().GetLocalPlayer ().mana.AddToPool (Mana.MANATYPE.BLOOD, totalMana);
	}
	public void RemoveBlood(Object target, params object[] args) {
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.BLOOD);
	}
	public void RemoveAllBlood(Object target, params object[] args) {
		int totalPool = Match.Get ().GetLocalPlayer ().mana.arManaPool [(int)Mana.MANATYPE.BLOOD];
		Match.Get ().GetLocalPlayer ().mana.RemoveFromPool (Mana.MANATYPE.BLOOD, totalPool);
	}

	public void AddAll(Object target, params object[] args) {
		AddAllPhysical (target);
		AddAllMental (target);
		AddAllEnergy (target);
		AddAllBlood (target);
	}

	public void RemoveAll(Object target, params object[] args) {
		RemoveAllPhysical (target);
		RemoveAllMental (target);
		RemoveAllEnergy (target);
		RemoveAllBlood (target);
	}
}