using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will need states for each of the possible types that are targettable:
// Can target Chars, Locations, Mana Type, Nothing(?), Abilities
public class StateTarget {

	public ContTarget contTarg;

	// Note that this isn't actually the normal Observer method, but will be called in the same way
	public virtual void UpdateObs (string eventType, Object target, params object[] args);


	public virtual void OnEnter ();
	public virtual void OnLeave ();


	public StateTarget(ContTarget _contTarg){
		contTarg = _contTarg;
	}
}
