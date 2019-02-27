using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will need states for each of the possible types that are targettable:
// Can target Chars, (Allies/Any) or Nothing
abstract public class StateTarget {

	public InputHuman inputHuman;

	public virtual void OnEnter (){}
	public virtual void OnLeave (){}


	public StateTarget(InputHuman _inputHuman) {
		inputHuman = _inputHuman;
	}
}
