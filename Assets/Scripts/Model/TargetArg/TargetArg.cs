using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetArg {

	public Chr chrOwner;

	public abstract bool WouldBeLegal(int indexTarget);

	public void setOwner(Chr _chrOwner){
		chrOwner = _chrOwner;
	}


}
