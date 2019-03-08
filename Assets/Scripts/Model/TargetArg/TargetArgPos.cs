using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArgPos : TargetArg {

	public delegate bool funcLegalPos (Chr own, Vector3 v3Tar);
	funcLegalPos fLegalCheck;

    public TargetArgPos(funcLegalPos _fLegalCheck){
		fLegalCheck = _fLegalCheck;

	}

    public override bool WouldBeLegal(int indexTarget){
		return fLegalCheck (chrOwner, Vector3.zero);
	}


}
