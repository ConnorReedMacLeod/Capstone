using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will need states for each of the possible types that are targettable:
// Can target Chars, Locations, Mana Type, Nothing(?), Abilities
public class StateTarget {

	public ContTarget contTarg;

	public virtual void OnEnter(){
		//what happens when you enter this state
	}

	public virtual void OnLeave(){
		//what happens when you leave this state
	}

	public virtual void OnClickArena(Vector3 pos){
		//requires the position that was clicked

	}

	public virtual void OnClickAct(Character chr, int idAct){

	}

	public virtual void OnClickChr(Character chr, Vector3 pos){
		//requires the selected character and it's position in the arena

	}

	public StateTarget(ContTarget _contTarg){
		contTarg = _contTarg;
	}
}
