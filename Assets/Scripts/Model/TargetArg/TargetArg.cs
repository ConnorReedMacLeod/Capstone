using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetArg {

	public Character chrOwner;

	public abstract bool VerifyLegal();

	public void setOwner(Character _chrOwner){
		chrOwner = _chrOwner;
	}

	public abstract void Reset ();

}
