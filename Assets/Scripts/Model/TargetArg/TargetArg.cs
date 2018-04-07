using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetArg {

	public Chr chrOwner;

	public abstract bool VerifyLegal();

	public void setOwner(Chr _chrOwner){
		chrOwner = _chrOwner;
	}

	public abstract void Reset ();

}
