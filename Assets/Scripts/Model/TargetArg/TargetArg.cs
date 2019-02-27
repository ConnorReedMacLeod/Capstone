using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetArg {

	public Chr chrOwner;

    public abstract bool CurrentlyLegal();
	public abstract bool VerifyLegal(int indexTarget);

	public void setOwner(Chr _chrOwner){
		chrOwner = _chrOwner;
	}

    //Let each type of target decide how to interpret a given index
    public abstract void SetTarget(int indexTarget);

	public abstract void Reset ();

}
