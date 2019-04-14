using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will need states for each of the possible types that are targettable:
// Can target Chars, (Allies/Any) or Nothing
abstract public class StateTarget {

	public virtual void OnEnter (){}
	public virtual void OnLeave (){}

}
